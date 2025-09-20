// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Romantic Dating App JavaScript Features

document.addEventListener('DOMContentLoaded', function() {
    // Add heart click effects
    const addHeartClickEffect = (element) => {
        element.addEventListener('click', function(e) {
            const heart = document.createElement('div');
            heart.innerHTML = '💖';
            heart.className = 'fixed text-4xl pointer-events-none z-50 animate-bounce';
            heart.style.left = (e.clientX - 20) + 'px';
            heart.style.top = (e.clientY - 20) + 'px';
            document.body.appendChild(heart);

            setTimeout(() => {
                heart.remove();
            }, 1000);
        });
    };

    // Apply heart click effect to romantic buttons
    document.querySelectorAll('.interest-button, .sendMessageBtn, [id*="send"]').forEach(button => {
        addHeartClickEffect(button);
    });

    // Add sparkle effect to profile cards on hover
    document.querySelectorAll('.user-card').forEach(card => {
        card.addEventListener('mouseenter', function() {
            const sparkle = document.createElement('div');
            sparkle.innerHTML = '✨';
            sparkle.className = 'absolute top-2 right-2 text-2xl animate-pulse';
            this.appendChild(sparkle);

            setTimeout(() => {
                sparkle.remove();
            }, 2000);
        });
    });

    // Romantic loading animation for forms
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function() {
            const button = form.querySelector('button[type="submit"]');
            if (button) {
                const originalText = button.innerHTML;
                button.innerHTML = '<i class="fas fa-heart fa-spin mr-2"></i>Procesando...';
                button.disabled = true;

                setTimeout(() => {
                    button.innerHTML = originalText;
                    button.disabled = false;
                }, 3000);
            }
        });
    });

    // Add floating hearts background effect
    let heartInterval;
    const createFloatingHeart = () => {
        const heart = document.createElement('div');
        heart.textContent = '💕'; // Use textContent instead of innerHTML for security
        heart.className = 'fixed text-2xl opacity-20 pointer-events-none animate-bounce';
        heart.style.left = Math.random() * 100 + 'vw';
        heart.style.top = '100vh';
        heart.style.animationDuration = (Math.random() * 5 + 3) + 's';
        document.body.appendChild(heart);

        setTimeout(() => {
            if (heart.parentNode) {
                heart.remove();
            }
        }, 8000);
    };

    // Create floating hearts every 8 seconds
    heartInterval = setInterval(createFloatingHeart, 8000);

    // Cleanup interval when page unloads
    window.addEventListener('beforeunload', () => {
        if (heartInterval) {
            clearInterval(heartInterval);
        }
    });

    // Romantic toast notifications
    window.showRomanticToast = function(message, type = 'love') {
        const toast = document.createElement('div');
        let bgColor = 'bg-pink-500';
        let icon = 'fas fa-heart';

        if (type === 'success') {
            bgColor = 'bg-green-500';
            icon = 'fas fa-check-circle';
        } else if (type === 'error') {
            bgColor = 'bg-red-500';
            icon = 'fas fa-exclamation-circle';
        }

        toast.className = `fixed top-4 right-4 z-50 p-4 rounded-lg shadow-lg text-white ${bgColor} transform translate-x-full transition-transform duration-300 flex items-center space-x-2`;

        // Create icon element
        const iconElement = document.createElement('i');
        iconElement.className = icon;

        // Create message span
        const messageSpan = document.createElement('span');
        messageSpan.textContent = message;

        // Append elements
        toast.appendChild(iconElement);
        toast.appendChild(messageSpan);

        document.body.appendChild(toast);

        setTimeout(() => {
            toast.classList.remove('translate-x-full');
        }, 100);

        setTimeout(() => {
            toast.classList.add('translate-x-full');
            setTimeout(() => {
                if (toast.parentNode) {
                    toast.remove();
                }
            }, 300);
        }, 3000);
    };

    // Auto-hide alerts after 5 seconds
    document.querySelectorAll('.alert').forEach(alert => {
        setTimeout(() => {
            alert.style.opacity = '0';
            setTimeout(() => alert.remove(), 300);
        }, 5000);
    });
});
