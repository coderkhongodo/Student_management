/**
 * STUDENT MANAGEMENT SYSTEM - DESIGN SYSTEM JAVASCRIPT
 * Modern, Consistent, Accessible UI Framework
 * 
 * This file contains global JavaScript utilities and components
 * that are used across the entire application.
 */

// Global Design System Object
window.DesignSystem = (function () {
    'use strict';

    // Configuration
    const config = {
        animationDuration: 250,
        debounceDelay: 300,
        breakpoints: {
            sm: 576,
            md: 768,
            lg: 992,
            xl: 1200,
            xxl: 1400
        }
    };

    // Utility Functions
    const utils = {
        // Debounce function for performance
        debounce: function (func, wait) {
            let timeout;
            return function executedFunction(...args) {
                const later = () => {
                    clearTimeout(timeout);
                    func(...args);
                };
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
            };
        },

        // Throttle function for scroll events
        throttle: function (func, limit) {
            let inThrottle;
            return function () {
                const args = arguments;
                const context = this;
                if (!inThrottle) {
                    func.apply(context, args);
                    inThrottle = true;
                    setTimeout(() => inThrottle = false, limit);
                }
            };
        },

        // Get current breakpoint
        getCurrentBreakpoint: function () {
            const width = window.innerWidth;
            if (width >= config.breakpoints.xxl) return 'xxl';
            if (width >= config.breakpoints.xl) return 'xl';
            if (width >= config.breakpoints.lg) return 'lg';
            if (width >= config.breakpoints.md) return 'md';
            if (width >= config.breakpoints.sm) return 'sm';
            return 'xs';
        },

        // Check if element is in viewport
        isInViewport: function (element) {
            const rect = element.getBoundingClientRect();
            return (
                rect.top >= 0 &&
                rect.left >= 0 &&
                rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
                rect.right <= (window.innerWidth || document.documentElement.clientWidth)
            );
        },

        // Smooth scroll to element
        scrollToElement: function (element, offset = 0) {
            const elementPosition = element.getBoundingClientRect().top;
            const offsetPosition = elementPosition + window.pageYOffset - offset;

            window.scrollTo({
                top: offsetPosition,
                behavior: 'smooth'
            });
        },

        // Generate unique ID
        generateId: function (prefix = 'ds') {
            return prefix + '_' + Math.random().toString(36).substr(2, 9);
        }
    };

    // Animation System
    const animations = {
        // Fade in animation
        fadeIn: function (element, duration = config.animationDuration) {
            element.style.opacity = '0';
            element.style.display = 'block';

            const start = performance.now();

            function animate(currentTime) {
                const elapsed = currentTime - start;
                const progress = Math.min(elapsed / duration, 1);

                element.style.opacity = progress;

                if (progress < 1) {
                    requestAnimationFrame(animate);
                }
            }

            requestAnimationFrame(animate);
        },

        // Slide down animation
        slideDown: function (element, duration = config.animationDuration) {
            element.style.height = '0';
            element.style.overflow = 'hidden';
            element.style.display = 'block';

            const targetHeight = element.scrollHeight;
            const start = performance.now();

            function animate(currentTime) {
                const elapsed = currentTime - start;
                const progress = Math.min(elapsed / duration, 1);

                element.style.height = (targetHeight * progress) + 'px';

                if (progress >= 1) {
                    element.style.height = '';
                    element.style.overflow = '';
                } else {
                    requestAnimationFrame(animate);
                }
            }

            requestAnimationFrame(animate);
        },

        // Slide up animation
        slideUp: function (element, duration = config.animationDuration) {
            const startHeight = element.offsetHeight;
            const start = performance.now();

            element.style.overflow = 'hidden';

            function animate(currentTime) {
                const elapsed = currentTime - start;
                const progress = Math.min(elapsed / duration, 1);

                element.style.height = (startHeight * (1 - progress)) + 'px';

                if (progress >= 1) {
                    element.style.display = 'none';
                    element.style.height = '';
                    element.style.overflow = '';
                } else {
                    requestAnimationFrame(animate);
                }
            }

            requestAnimationFrame(animate);
        }
    };

    // Notification System
    const notifications = {
        container: null,

        init: function () {
            if (!this.container) {
                this.container = document.createElement('div');
                this.container.className = 'notification-container';
                this.container.style.cssText = `
                    position: fixed;
                    top: 20px;
                    right: 20px;
                    z-index: 9999;
                    max-width: 400px;
                `;
                document.body.appendChild(this.container);
            }
        },

        show: function (message, type = 'info', duration = 5000) {
            this.init();

            const notification = document.createElement('div');
            notification.className = `alert alert-${type} alert-dismissible fade show notification-item`;
            notification.style.cssText = `
                margin-bottom: 10px;
                box-shadow: 0 4px 12px rgba(0,0,0,0.15);
                border: none;
                border-radius: 8px;
            `;

            notification.innerHTML = `
                <div class="d-flex align-items-center">
                    <i class="fas fa-${this.getIcon(type)} me-2"></i>
                    <span>${message}</span>
                    <button type="button" class="btn-close ms-auto" data-bs-dismiss="alert"></button>
                </div>
            `;

            this.container.appendChild(notification);

            // Auto remove after duration
            if (duration > 0) {
                setTimeout(() => {
                    if (notification.parentNode) {
                        notification.remove();
                    }
                }, duration);
            }

            return notification;
        },

        getIcon: function (type) {
            const icons = {
                success: 'check-circle',
                danger: 'exclamation-triangle',
                warning: 'exclamation-circle',
                info: 'info-circle'
            };
            return icons[type] || 'info-circle';
        }
    };

    // Form Validation Enhancement
    const forms = {
        init: function () {
            // Add real-time validation to all forms
            document.querySelectorAll('form').forEach(form => {
                this.enhanceForm(form);
            });
        },

        enhanceForm: function (form) {
            const inputs = form.querySelectorAll('input, select, textarea');

            inputs.forEach(input => {
                // Add floating label effect
                this.addFloatingLabel(input);

                // Add validation feedback
                input.addEventListener('blur', () => {
                    this.validateField(input);
                });

                input.addEventListener('input', utils.debounce(() => {
                    this.validateField(input);
                }, config.debounceDelay));
            });
        },

        addFloatingLabel: function (input) {
            if (input.type === 'hidden' || input.type === 'submit') return;

            const wrapper = input.closest('.form-floating');
            if (!wrapper) return;

            const label = wrapper.querySelector('label');
            if (!label) return;

            // Add focus/blur handlers for floating effect
            input.addEventListener('focus', () => {
                wrapper.classList.add('focused');
            });

            input.addEventListener('blur', () => {
                if (!input.value) {
                    wrapper.classList.remove('focused');
                }
            });

            // Check initial state
            if (input.value) {
                wrapper.classList.add('focused');
            }
        },

        validateField: function (input) {
            const isValid = input.checkValidity();
            const feedback = input.parentNode.querySelector('.invalid-feedback');

            if (isValid) {
                input.classList.remove('is-invalid');
                input.classList.add('is-valid');
            } else {
                input.classList.remove('is-valid');
                input.classList.add('is-invalid');

                if (feedback) {
                    feedback.textContent = input.validationMessage;
                }
            }
        }
    };

    // Loading States
    const loading = {
        show: function (element, text = 'Loading...') {
            const loader = document.createElement('div');
            loader.className = 'loading-overlay';
            loader.innerHTML = `
                <div class="loading-content">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">${text}</span>
                    </div>
                    <div class="loading-text mt-2">${text}</div>
                </div>
            `;

            loader.style.cssText = `
                position: absolute;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                background: rgba(255, 255, 255, 0.9);
                display: flex;
                align-items: center;
                justify-content: center;
                z-index: 1000;
                border-radius: inherit;
            `;

            element.style.position = 'relative';
            element.appendChild(loader);

            return loader;
        },

        hide: function (element) {
            const loader = element.querySelector('.loading-overlay');
            if (loader) {
                loader.remove();
            }
        }
    };

    // Public API
    return {
        config: config,
        utils: utils,
        animations: animations,
        notifications: notifications,
        forms: forms,
        loading: loading,

        // Initialize the design system
        init: function () {
            console.log('🎨 Design System initialized');

            // Initialize components
            this.forms.init();
            this.notifications.init();

            // Add global event listeners
            this.initGlobalEvents();
        },

        initGlobalEvents: function () {
            // Smooth scroll for anchor links
            document.addEventListener('click', (e) => {
                const link = e.target.closest('a[href^="#"]');
                if (link) {
                    e.preventDefault();
                    const target = document.querySelector(link.getAttribute('href'));
                    if (target) {
                        this.utils.scrollToElement(target, 80);
                    }
                }
            });

            // Handle responsive navigation
            window.addEventListener('resize', utils.throttle(() => {
                const currentBreakpoint = utils.getCurrentBreakpoint();
                document.body.setAttribute('data-breakpoint', currentBreakpoint);
            }, 100));

            // Initial breakpoint set
            document.body.setAttribute('data-breakpoint', utils.getCurrentBreakpoint());
        },

        // Modal utilities
        modal: {
            show: function (content, options = {}) {
                const defaults = {
                    title: 'Modal',
                    size: 'modal-lg',
                    backdrop: true,
                    keyboard: true,
                    showCloseButton: true
                };

                const settings = Object.assign({}, defaults, options);

                const modalId = utils.generateId('modal');
                const modalHtml = `
                    <div class="modal fade" id="${modalId}" tabindex="-1" aria-hidden="true">
                        <div class="modal-dialog ${settings.size}">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title">${settings.title}</h5>
                                    ${settings.showCloseButton ? '<button type="button" class="btn-close" data-bs-dismiss="modal"></button>' : ''}
                                </div>
                                <div class="modal-body">
                                    ${content}
                                </div>
                            </div>
                        </div>
                    </div>
                `;

                document.body.insertAdjacentHTML('beforeend', modalHtml);
                const modalElement = document.getElementById(modalId);
                const modal = new bootstrap.Modal(modalElement, {
                    backdrop: settings.backdrop,
                    keyboard: settings.keyboard
                });

                // Clean up when modal is hidden
                modalElement.addEventListener('hidden.bs.modal', () => {
                    modalElement.remove();
                });

                modal.show();
                return modal;
            },

            confirm: function (message, title = 'Xác nhận') {
                return new Promise((resolve) => {
                    const content = `
                        <p>${message}</p>
                        <div class="d-flex justify-content-end gap-2 mt-3">
                            <button type="button" class="btn btn-secondary" data-action="cancel">Hủy</button>
                            <button type="button" class="btn btn-primary" data-action="confirm">Xác nhận</button>
                        </div>
                    `;

                    const modal = this.show(content, { title, showCloseButton: false });

                    document.addEventListener('click', function handler(e) {
                        if (e.target.dataset.action === 'confirm') {
                            modal.hide();
                            document.removeEventListener('click', handler);
                            resolve(true);
                        } else if (e.target.dataset.action === 'cancel') {
                            modal.hide();
                            document.removeEventListener('click', handler);
                            resolve(false);
                        }
                    });
                });
            }
        },

        // Table utilities
        table: {
            makeResponsive: function (table) {
                if (!table.closest('.table-responsive')) {
                    const wrapper = document.createElement('div');
                    wrapper.className = 'table-responsive';
                    table.parentNode.insertBefore(wrapper, table);
                    wrapper.appendChild(table);
                }
            },

            addSearch: function (table, searchInput) {
                const rows = table.querySelectorAll('tbody tr');

                searchInput.addEventListener('input', utils.debounce((e) => {
                    const searchTerm = e.target.value.toLowerCase();

                    rows.forEach(row => {
                        const text = row.textContent.toLowerCase();
                        const shouldShow = text.includes(searchTerm);
                        row.style.display = shouldShow ? '' : 'none';
                    });
                }, config.debounceDelay));
            }
        },

        // Card utilities
        card: {
            addHoverEffect: function (card) {
                card.addEventListener('mouseenter', () => {
                    card.style.transform = 'translateY(-2px)';
                    card.style.boxShadow = '0 8px 25px rgba(0,0,0,0.15)';
                });

                card.addEventListener('mouseleave', () => {
                    card.style.transform = '';
                    card.style.boxShadow = '';
                });
            }
        }
    };
})();
