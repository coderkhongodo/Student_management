// Accessibility Enhancement JavaScript
// WCAG 2.1 AA Compliance Support

(function() {
    'use strict';

    // Initialize accessibility features when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        initializeAccessibility();
    });

    function initializeAccessibility() {
        // Detect keyboard navigation
        detectKeyboardNavigation();
        
        // Enhance form accessibility
        enhanceFormAccessibility();
        
        // Enhance table accessibility
        enhanceTableAccessibility();
        
        // Enhance modal accessibility
        enhanceModalAccessibility();
        
        // Add ARIA live regions
        addLiveRegions();
        
        // Handle focus management
        manageFocus();
        
        // Add keyboard shortcuts
        addKeyboardShortcuts();
        
        // Announce page changes for screen readers
        announcePageChanges();
    }

    // Detect if user is navigating with keyboard
    function detectKeyboardNavigation() {
        let isKeyboardUser = false;
        
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Tab') {
                isKeyboardUser = true;
                document.body.classList.add('keyboard-navigation-active');
            }
        });
        
        document.addEventListener('mousedown', function() {
            if (isKeyboardUser) {
                isKeyboardUser = false;
                document.body.classList.remove('keyboard-navigation-active');
            }
        });
    }

    // Enhance form accessibility
    function enhanceFormAccessibility() {
        // Add required indicators
        const requiredInputs = document.querySelectorAll('input[required], select[required], textarea[required]');
        requiredInputs.forEach(function(input) {
            const label = document.querySelector(`label[for="${input.id}"]`);
            if (label && !label.classList.contains('required')) {
                label.classList.add('required');
                label.setAttribute('aria-required', 'true');
            }
        });

        // Add error announcements
        const forms = document.querySelectorAll('form');
        forms.forEach(function(form) {
            form.addEventListener('submit', function(e) {
                const errors = form.querySelectorAll('.form-error:not(:empty)');
                if (errors.length > 0) {
                    announceToScreenReader(`Form has ${errors.length} error${errors.length > 1 ? 's' : ''}. Please review and correct.`);
                }
            });
        });

        // Real-time validation feedback
        const inputs = document.querySelectorAll('input, select, textarea');
        inputs.forEach(function(input) {
            input.addEventListener('blur', function() {
                validateInput(input);
            });
        });
    }

    // Validate individual input
    function validateInput(input) {
        const isValid = input.checkValidity();
        const errorElement = input.parentNode.querySelector('.form-error');
        
        if (!isValid) {
            input.setAttribute('aria-invalid', 'true');
            if (errorElement) {
                errorElement.textContent = input.validationMessage;
                input.setAttribute('aria-describedby', errorElement.id || 'error-' + input.id);
            }
        } else {
            input.setAttribute('aria-invalid', 'false');
            if (errorElement) {
                errorElement.textContent = '';
            }
        }
    }

    // Enhance table accessibility
    function enhanceTableAccessibility() {
        const tables = document.querySelectorAll('.table');
        tables.forEach(function(table) {
            // Add table caption if missing
            if (!table.querySelector('caption')) {
                const caption = document.createElement('caption');
                caption.textContent = 'Data table';
                caption.className = 'sr-only';
                table.insertBefore(caption, table.firstChild);
            }

            // Make rows focusable and add keyboard navigation
            const rows = table.querySelectorAll('tbody tr');
            rows.forEach(function(row, index) {
                row.setAttribute('tabindex', '0');
                row.setAttribute('role', 'row');
                
                row.addEventListener('keydown', function(e) {
                    let targetRow = null;
                    
                    switch(e.key) {
                        case 'ArrowDown':
                            targetRow = rows[index + 1];
                            break;
                        case 'ArrowUp':
                            targetRow = rows[index - 1];
                            break;
                        case 'Home':
                            targetRow = rows[0];
                            break;
                        case 'End':
                            targetRow = rows[rows.length - 1];
                            break;
                    }
                    
                    if (targetRow) {
                        e.preventDefault();
                        targetRow.focus();
                    }
                });
            });
        });
    }

    // Enhance modal accessibility
    function enhanceModalAccessibility() {
        const modals = document.querySelectorAll('.modal');
        modals.forEach(function(modal) {
            modal.addEventListener('shown.bs.modal', function() {
                // Focus first focusable element
                const focusableElements = modal.querySelectorAll(
                    'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
                );
                if (focusableElements.length > 0) {
                    focusableElements[0].focus();
                }
                
                // Trap focus within modal
                trapFocus(modal);
            });
            
            modal.addEventListener('hidden.bs.modal', function() {
                // Return focus to trigger element
                const trigger = document.querySelector(`[data-bs-target="#${modal.id}"]`);
                if (trigger) {
                    trigger.focus();
                }
            });
        });
    }

    // Trap focus within element
    function trapFocus(element) {
        const focusableElements = element.querySelectorAll(
            'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
        );
        const firstElement = focusableElements[0];
        const lastElement = focusableElements[focusableElements.length - 1];

        element.addEventListener('keydown', function(e) {
            if (e.key === 'Tab') {
                if (e.shiftKey) {
                    if (document.activeElement === firstElement) {
                        e.preventDefault();
                        lastElement.focus();
                    }
                } else {
                    if (document.activeElement === lastElement) {
                        e.preventDefault();
                        firstElement.focus();
                    }
                }
            }
        });
    }

    // Add ARIA live regions for dynamic content
    function addLiveRegions() {
        // Create live region for announcements
        const liveRegion = document.createElement('div');
        liveRegion.id = 'live-region';
        liveRegion.setAttribute('aria-live', 'polite');
        liveRegion.setAttribute('aria-atomic', 'true');
        liveRegion.className = 'sr-only';
        document.body.appendChild(liveRegion);

        // Create assertive live region for urgent announcements
        const assertiveLiveRegion = document.createElement('div');
        assertiveLiveRegion.id = 'assertive-live-region';
        assertiveLiveRegion.setAttribute('aria-live', 'assertive');
        assertiveLiveRegion.setAttribute('aria-atomic', 'true');
        assertiveLiveRegion.className = 'sr-only';
        document.body.appendChild(assertiveLiveRegion);
    }

    // Announce messages to screen readers
    function announceToScreenReader(message, isUrgent = false) {
        const liveRegion = document.getElementById(isUrgent ? 'assertive-live-region' : 'live-region');
        if (liveRegion) {
            liveRegion.textContent = message;
            
            // Clear after announcement
            setTimeout(function() {
                liveRegion.textContent = '';
            }, 1000);
        }
    }

    // Manage focus for dynamic content
    function manageFocus() {
        // Handle AJAX content updates
        document.addEventListener('contentUpdated', function(e) {
            const newContent = e.detail.element;
            if (newContent) {
                // Focus first heading or focusable element in new content
                const heading = newContent.querySelector('h1, h2, h3, h4, h5, h6');
                const focusable = newContent.querySelector('button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])');
                
                if (heading) {
                    heading.setAttribute('tabindex', '-1');
                    heading.focus();
                } else if (focusable) {
                    focusable.focus();
                }
            }
        });
    }

    // Add keyboard shortcuts
    function addKeyboardShortcuts() {
        document.addEventListener('keydown', function(e) {
            // Alt + M: Go to main content
            if (e.altKey && e.key === 'm') {
                e.preventDefault();
                const mainContent = document.getElementById('main-content');
                if (mainContent) {
                    mainContent.focus();
                    announceToScreenReader('Jumped to main content');
                }
            }
            
            // Alt + N: Go to navigation
            if (e.altKey && e.key === 'n') {
                e.preventDefault();
                const navigation = document.getElementById('navigation');
                if (navigation) {
                    const firstLink = navigation.querySelector('a, button');
                    if (firstLink) {
                        firstLink.focus();
                        announceToScreenReader('Jumped to navigation');
                    }
                }
            }
            
            // Escape: Close modals, dropdowns, etc.
            if (e.key === 'Escape') {
                const openModal = document.querySelector('.modal.show');
                if (openModal) {
                    const closeButton = openModal.querySelector('.btn-close');
                    if (closeButton) {
                        closeButton.click();
                    }
                }
                
                const openDropdown = document.querySelector('.dropdown-menu.show');
                if (openDropdown) {
                    const toggle = document.querySelector('[data-bs-toggle="dropdown"][aria-expanded="true"]');
                    if (toggle) {
                        toggle.click();
                    }
                }
            }
        });
    }

    // Announce page changes for single-page applications
    function announcePageChanges() {
        // Monitor for page title changes
        const observer = new MutationObserver(function(mutations) {
            mutations.forEach(function(mutation) {
                if (mutation.type === 'childList' && mutation.target.tagName === 'TITLE') {
                    announceToScreenReader(`Page changed to ${document.title}`);
                }
            });
        });
        
        const titleElement = document.querySelector('title');
        if (titleElement) {
            observer.observe(titleElement, { childList: true });
        }
    }

    // Expose utility functions globally
    window.AccessibilityUtils = {
        announceToScreenReader: announceToScreenReader,
        validateInput: validateInput,
        trapFocus: trapFocus
    };

})();
