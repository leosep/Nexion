using DatingApp.Core.Interfaces;
using DatingApp.Core.Models;
using DatingApp.Web.Hubs;
using DatingApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

namespace DatingApp.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMatchService _matchService;
        private readonly IHubContext<DatingHub> _hubContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(IUserService userService, IMatchService matchService, IHubContext<DatingHub> hubContext, IWebHostEnvironment webHostEnvironment)
        {
            _userService = userService;
            _matchService = matchService;
            _hubContext = hubContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string interests, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var users = await _userService.GetUsersByPreferencesAsync(currentUserId, interests, page, pageSize);
            var userViewModels = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Username = u.Username,
                Bio = u.Bio,
                Interests = u.Interests,
                ProfilePhotoUrls = u.ProfilePhotoUrls ?? new List<string>(),
                PromptAnswers = u.PromptAnswers ?? new Dictionary<string, string>()
            });
            return View(userViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Descubrir([FromQuery] string interests, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var users = await _userService.GetUsersByPreferencesAsync(currentUserId, interests, page, pageSize);
            var userViewModels = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Username = u.Username,
                Bio = u.Bio,
                Interests = u.Interests,
                ProfilePhotoUrls = u.ProfilePhotoUrls ?? new List<string>(),
                PromptAnswers = u.PromptAnswers ?? new Dictionary<string, string>()
            });
            return View(userViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> AddInterest([FromBody] InterestRequest request)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isMatch = await _matchService.AddInterestAsync(currentUserId, request.TargetUserId);

            if (isMatch)
            {
                var currentUser = await _userService.GetUserByIdAsync(currentUserId);
                var targetUser = await _userService.GetUserByIdAsync(request.TargetUserId);
                await _hubContext.Clients.User(currentUserId.ToString()).SendAsync("ReceiveMatch", targetUser.Username);
                await _hubContext.Clients.User(request.TargetUserId.ToString()).SendAsync("ReceiveMatch", currentUser.Username);
            }

            return Json(new { success = true, isMatch });
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new ProfileViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Bio = user.Bio,
                Interests = user.Interests,
                // Deserializa la cadena JSON para la vista
                ProfilePhotoUrls = string.IsNullOrEmpty(user.ProfilePhotoUrlsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(user.ProfilePhotoUrlsJson),
                PromptAnswers = string.IsNullOrEmpty(user.PromptAnswersJson) ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(user.PromptAnswersJson),
                Gender = user.Gender,
                LookingFor = user.LookingFor,
                MinAge = user.MinAge,
                MaxAge = user.MaxAge,
                Country = user.Country,
                City = user.City,
                DateOfBirth = user.DateOfBirth
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var user = await _userService.GetUserByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Username = model.Username;
            user.Bio = model.Bio;
            user.Interests = model.Interests;
            user.PromptAnswers = model.PromptAnswers; // El repositorio se encargará de serializar
            user.Gender = model.Gender;
            user.LookingFor = model.LookingFor;
            user.MinAge = model.MinAge;
            user.MaxAge = model.MaxAge;
            user.Country = model.Country;
            user.City = model.City;
            user.DateOfBirth = model.DateOfBirth;

            if (model.ProfilePhotos != null && model.ProfilePhotos.Count > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                // Deserializa el JSON actual a una lista de C#
                var photoUrls = string.IsNullOrEmpty(user.ProfilePhotoUrlsJson)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(user.ProfilePhotoUrlsJson);

                foreach (var file in model.ProfilePhotos)
                {
                    if (file.Length > 0)
                    {
                        // Security: Validate file size (max 5MB)
                        if (file.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("ProfilePhotos", "File size must be less than 5MB");
                            continue;
                        }

                        // Security: Validate file type
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("ProfilePhotos", "Only image files (.jpg, .jpeg, .png, .gif) are allowed");
                            continue;
                        }

                        // Security: Validate file content type
                        if (!file.ContentType.StartsWith("image/"))
                        {
                            ModelState.AddModelError("ProfilePhotos", "Invalid file type");
                            continue;
                        }

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        try
                        {
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            string photoUrl = "/uploads/" + uniqueFileName;
                            photoUrls.Add(photoUrl);
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("ProfilePhotos", "Error uploading file: " + file.FileName);
                        }
                    }
                }

                // Asigna la lista al modelo del usuario (el repositorio se encargará de serializar)
                user.ProfilePhotoUrls = photoUrls;
            }

            try
            {
                await _userService.UpdateUserProfileAsync(user);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Profile", model);
            }

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto([FromBody] PhotoDeleteRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Deserializa el JSON a una lista
            var photoUrls = string.IsNullOrEmpty(user.ProfilePhotoUrlsJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(user.ProfilePhotoUrlsJson);

            if (photoUrls == null || photoUrls.Count == 0)
            {
                return Json(new { success = false, message = "El usuario no tiene fotos." });
            }

            // Convert full URL to relative path if necessary
            var relativePhotoUrl = request.PhotoUrl;
            if (relativePhotoUrl.StartsWith(Request.Scheme + "://" + Request.Host))
            {
                relativePhotoUrl = relativePhotoUrl.Substring((Request.Scheme + "://" + Request.Host).Length);
            }

            var photoToRemove = photoUrls.FirstOrDefault(url => url.Equals(relativePhotoUrl, StringComparison.OrdinalIgnoreCase));

            if (photoToRemove == null)
            {
                return Json(new { success = false, message = "La foto no pertenece al usuario o no existe." });
            }

            try
            {
                var relativePath = photoToRemove.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "No se pudo eliminar el archivo físico." });
            }

            // Remueve la foto de la lista en memoria.
            photoUrls.Remove(photoToRemove);

            // Asigna la lista al modelo del usuario (el repositorio se encargará de serializar)
            user.ProfilePhotoUrls = photoUrls;

            await _userService.UpdateUserProfileAsync(user);

            return Json(photoUrls);
        }
    }

    public class InterestRequest
    {
        public int TargetUserId { get; set; }
    }

    public class PhotoDeleteRequest
    {
        public string PhotoUrl { get; set; }
    }
}