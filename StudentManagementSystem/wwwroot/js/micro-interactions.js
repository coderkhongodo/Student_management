// Micro-interactions and UI Polish JavaScript
(function() {
    'use strict';

    // Initialize micro-interactions when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        initializeMicroInteractions();
    });

    function initializeMicroInteractions() {
        // Initialize scroll reveal animations
        initializeScrollReveal();
        
        // Initialize button ripple effects
        initializeRippleEffects();
        
        // Initialize loading states
        initializeLoadingStates();
        
        // Initialize form enhancements
        initializeFormEnhancements();
        
        // Initialize card interactions
        initializeCardInteractions();
        
        // Initialize notification system
        initializeNotifications();
        
        // Initialize performance monitoring
        initializePerformanceMode();
    }

    // Scroll reveal animations
    function initializeScrollReveal() {
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        const observer = new IntersectionObserver(function(entries) {
            entries.forEach(function(entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('revealed');
                    observer.unobserve(entry.target);
                }
            });
        }, observerOptions);

        // Observe elements with scroll-reveal class
        document.querySelectorAll('.scroll-reveal').forEach(function(element) {
            observer.observe(element);
        });

        // Stagger animations for lists
        document.querySelectorAll('.stagger-container').forEach(function(container) {
            const items = container.querySelectorAll('.stagger-item');
            items.forEach(function(item, index) {
                item.style.animationDelay = (index * 0.1) + 's';
            });
        });
    }

    // Button ripple effects
    function initializeRippleEffects() {
        document.querySelectorAll('.btn').forEach(function(button) {
            button.addEventListener('click', function(e) {
                const ripple = document.createElement('span');
                const rect = button.getBoundingClientRect();
                const size = Math.max(rect.width, rect.height);
                const x = e.clientX - rect.left - size / 2;
                const y = e.clientY - rect.top - size / 2;
                
                ripple.style.width = ripple.style.height = size + 'px';
                ripple.style.left = x + 'px';
                ripple.style.top = y + 'px';
                ripple.classList.add('ripple');
                
                button.appendChild(ripple);
                
                setTimeout(function() {
                    ripple.remove();
                }, 600);
            });
        });
    }

    // Loading states
    function initializeLoadingStates() {
        // Auto-add loading state to forms on submit
        document.querySelectorAll('form').forEach(function(form) {
            form.addEventListener('submit', function() {
                const submitButton = form.querySelector('button[type="submit"], input[type="submit"]');
                if (submitButton) {
                    submitButton.classList.add('loading');
                    submitButton.disabled = true;
                }
            });
        });

        // AJAX loading states
        if (window.jQuery) {
            $(document).ajaxStart(function() {
                $('.ajax-loader').show();
            }).ajaxStop(function() {
                $('.ajax-loader').hide();
                $('.btn.loading').removeClass('loading').prop('disabled', false);
            });
        }
    }

    // Form enhancements
    function initializeFormEnhancements() {
        // Auto-resize textareas
        document.querySelectorAll('textarea').forEach(function(textarea) {
            textarea.addEventListener('input', function() {
                this.style.height = 'auto';
                this.style.height = this.scrollHeight + 'px';
            });
        });

        // Form validation feedback
        document.querySelectorAll('input, select, textarea').forEach(function(input) {
            input.addEventListener('blur', function() {
                validateInputWithAnimation(input);
            });

            input.addEventListener('input', function() {
                if (input.classList.contains('is-invalid')) {
                    validateInputWithAnimation(input);
                }
            });
        });

        // Auto-save functionality
        let autoSaveTimeout;
        document.querySelectorAll('[data-autosave]').forEach(function(input) {
            input.addEventListener('input', function() {
                clearTimeout(autoSaveTimeout);
                autoSaveTimeout = setTimeout(function() {
                    autoSaveForm(input.closest('form'));
                }, 2000);
            });
        });
    }

    function validateInputWithAnimation(input) {
        const isValid = input.checkValidity();
        
        if (isValid) {
            input.classList.remove('is-invalid');
            input.classList.add('is-valid');
            input.parentElement.classList.add('success-animation');
        } else {
            input.classList.remove('is-valid');
            input.classList.add('is-invalid');
            input.parentElement.classList.add('error-animation');
        }

        // Remove animation classes after animation completes
        setTimeout(function() {
            input.parentElement.classList.remove('success-animation', 'error-animation');
        }, 600);
    }

    function autoSaveForm(form) {
        if (!form) return;

        const formData = new FormData(form);
        const indicator = document.createElement('div');
        indicator.className = 'auto-save-indicator';
        indicator.innerHTML = '<i class="fas fa-save"></i> Auto-saved';
        indicator.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: #28a745;
            color: white;
            padding: 8px 16px;
            border-radius: 4px;
            z-index: 9999;
            font-size: 14px;
        `;

        document.body.appendChild(indicator);

        // Simulate auto-save (replace with actual AJAX call)
        setTimeout(function() {
            indicator.remove();
        }, 2000);
    }

    // Card interactions
    function initializeCardInteractions() {
        document.querySelectorAll('.card-clickable').forEach(function(card) {
            card.addEventListener('click', function(e) {
                if (e.target.closest('button, a, input, select, textarea')) {
                    return; // Don't trigger if clicking on interactive elements
                }

                const href = card.dataset.href;
                if (href) {
                    window.location.href = href;
                }
            });

            // Add keyboard support
            card.setAttribute('tabindex', '0');
            card.addEventListener('keydown', function(e) {
                if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    card.click();
                }
            });
        });
    }

    // Notification system
    function initializeNotifications() {
        window.showNotification = function(message, type = 'info', duration = 5000) {
            const notification = document.createElement('div');
            notification.className = `notification alert alert-${type} alert-dismissible fade show`;
            notification.innerHTML = `
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            `;
            notification.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 9999;
                min-width: 300px;
                max-width: 500px;
            `;

            document.body.appendChild(notification);

            // Auto-remove after duration
            setTimeout(function() {
                if (notification.parentElement) {
                    notification.remove();
                }
            }, duration);
        };

        // Success/error message animations
        document.querySelectorAll('.alert').forEach(function(alert) {
            alert.classList.add('scroll-reveal');
        });
    }

    // Performance mode for low-end devices
    function initializePerformanceMode() {
        // Detect low-end devices
        const isLowEndDevice = navigator.hardwareConcurrency <= 2 || 
                              navigator.deviceMemory <= 2 ||
                              /Android.*Chrome\/[.0-9]*\s/.test(navigator.userAgent);

        if (isLowEndDevice) {
            document.body.classList.add('performance-mode');
        }

        // Allow manual performance mode toggle
        window.togglePerformanceMode = function() {
            document.body.classList.toggle('performance-mode');
            localStorage.setItem('performanceMode', 
                document.body.classList.contains('performance-mode'));
        };

        // Restore performance mode setting
        if (localStorage.getItem('performanceMode') === 'true') {
            document.body.classList.add('performance-mode');
        }
    }

    // Utility functions
    window.MicroInteractions = {
        // Trigger success animation
        showSuccess: function(element) {
            element.classList.add('success-animation');
            setTimeout(function() {
                element.classList.remove('success-animation');
            }, 600);
        },

        // Trigger error animation
        showError: function(element) {
            element.classList.add('error-animation');
            setTimeout(function() {
                element.classList.remove('error-animation');
            }, 600);
        },

        // Add loading state to button
        setLoading: function(button, loading = true) {
            if (loading) {
                button.classList.add('loading');
                button.disabled = true;
            } else {
                button.classList.remove('loading');
                button.disabled = false;
            }
        },

        // Smooth scroll to element
        scrollTo: function(element, offset = 0) {
            const targetPosition = element.offsetTop - offset;
            window.scrollTo({
                top: targetPosition,
                behavior: 'smooth'
            });
        },

        // Animate number counting
        animateNumber: function(element, start, end, duration = 1000) {
            const range = end - start;
            const increment = range / (duration / 16);
            let current = start;

            const timer = setInterval(function() {
                current += increment;
                if (current >= end) {
                    current = end;
                    clearInterval(timer);
                }
                element.textContent = Math.floor(current);
            }, 16);
        },

        // Pulse animation
        pulse: function(element) {
            element.style.animation = 'pulse 0.6s ease';
            setTimeout(function() {
                element.style.animation = '';
            }, 600);
        }
    };

    // Expose notification function globally
    window.showNotification = window.showNotification || function() {};

})();
