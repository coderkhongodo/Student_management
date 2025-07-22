// Student UI Enhancements JavaScript

document.addEventListener('DOMContentLoaded', function() {
    initializeStudentUI();
});

function initializeStudentUI() {
    // Initialize all UI enhancements
    initializeAnimations();
    initializeToasts();
    initializeProgressBars();
    initializeCountdowns();
    initializeLoadingStates();
    initializeKeyboardShortcuts();
    initializeMobileOptimizations();
}

// Animation Enhancements
function initializeAnimations() {
    // Add entrance animations to cards
    const cards = document.querySelectorAll('.card');
    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            card.style.transition = 'all 0.6s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 100);
    });

    // Add hover effects to buttons
    const buttons = document.querySelectorAll('.btn');
    buttons.forEach(button => {
        button.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-2px)';
        });
        
        button.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0)';
        });
    });
}

// Toast Notification System
function initializeToasts() {
    // Create toast container if it doesn't exist
    if (!document.querySelector('.toast-container')) {
        const toastContainer = document.createElement('div');
        toastContainer.className = 'toast-container';
        document.body.appendChild(toastContainer);
    }
}

function showToast(message, type = 'info', duration = 5000) {
    const toastContainer = document.querySelector('.toast-container');
    const toastId = 'toast-' + Date.now();
    
    const toastHTML = `
        <div id="${toastId}" class="toast toast-${type}" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="toast-header">
                <i class="fas fa-${getToastIcon(type)} me-2"></i>
                <strong class="me-auto">${getToastTitle(type)}</strong>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
            </div>
            <div class="toast-body">
                ${message}
            </div>
        </div>
    `;
    
    toastContainer.insertAdjacentHTML('beforeend', toastHTML);
    
    const toastElement = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastElement, { delay: duration });
    toast.show();
    
    // Remove toast element after it's hidden
    toastElement.addEventListener('hidden.bs.toast', function() {
        this.remove();
    });
}

function getToastIcon(type) {
    const icons = {
        'success': 'check-circle',
        'error': 'exclamation-circle',
        'warning': 'exclamation-triangle',
        'info': 'info-circle'
    };
    return icons[type] || 'info-circle';
}

function getToastTitle(type) {
    const titles = {
        'success': 'Thành công',
        'error': 'Lỗi',
        'warning': 'Cảnh báo',
        'info': 'Thông báo'
    };
    return titles[type] || 'Thông báo';
}

// Progress Bar Animations
function initializeProgressBars() {
    const progressBars = document.querySelectorAll('.progress-bar');
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const progressBar = entry.target;
                const width = progressBar.style.width;
                progressBar.style.width = '0%';
                
                setTimeout(() => {
                    progressBar.style.transition = 'width 1.5s ease-in-out';
                    progressBar.style.width = width;
                }, 100);
                
                observer.unobserve(progressBar);
            }
        });
    });
    
    progressBars.forEach(bar => observer.observe(bar));
}

// Countdown Timers
function initializeCountdowns() {
    const countdownElements = document.querySelectorAll('[data-countdown]');
    
    countdownElements.forEach(element => {
        const targetDate = new Date(element.dataset.countdown);
        updateCountdown(element, targetDate);
        
        setInterval(() => {
            updateCountdown(element, targetDate);
        }, 1000);
    });
}

function updateCountdown(element, targetDate) {
    const now = new Date().getTime();
    const distance = targetDate.getTime() - now;
    
    if (distance < 0) {
        element.innerHTML = '<span class="text-danger">Đã hết hạn</span>';
        return;
    }
    
    const days = Math.floor(distance / (1000 * 60 * 60 * 24));
    const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((distance % (1000 * 60)) / 1000);
    
    let countdownText = '';
    if (days > 0) countdownText += `${days}d `;
    if (hours > 0) countdownText += `${hours}h `;
    if (minutes > 0) countdownText += `${minutes}m `;
    countdownText += `${seconds}s`;
    
    element.innerHTML = `<i class="fas fa-clock"></i> ${countdownText}`;
    
    // Add urgency styling
    if (distance < 3600000) { // Less than 1 hour
        element.className = 'text-danger fw-bold';
    } else if (distance < 86400000) { // Less than 1 day
        element.className = 'text-warning fw-bold';
    }
}

// Loading States
function initializeLoadingStates() {
    // Add loading states to forms
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function() {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn) {
                const originalText = submitBtn.innerHTML;
                submitBtn.innerHTML = '<span class="loading-spinner"></span> Đang xử lý...';
                submitBtn.disabled = true;
                
                // Re-enable after 10 seconds as fallback
                setTimeout(() => {
                    submitBtn.innerHTML = originalText;
                    submitBtn.disabled = false;
                }, 10000);
            }
        });
    });
    
    // Add loading states to AJAX links
    const ajaxLinks = document.querySelectorAll('[data-ajax="true"]');
    ajaxLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            showLoadingOverlay();
            
            // Simulate AJAX call
            setTimeout(() => {
                hideLoadingOverlay();
                showToast('Dữ liệu đã được tải', 'success');
            }, 1500);
        });
    });
}

function showLoadingOverlay() {
    const overlay = document.createElement('div');
    overlay.id = 'loading-overlay';
    overlay.innerHTML = `
        <div class="d-flex justify-content-center align-items-center h-100">
            <div class="text-center">
                <div class="loading-spinner mb-3" style="width: 40px; height: 40px;"></div>
                <p class="text-white">Đang tải...</p>
            </div>
        </div>
    `;
    overlay.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0,0,0,0.7);
        z-index: 9999;
        display: flex;
    `;
    document.body.appendChild(overlay);
}

function hideLoadingOverlay() {
    const overlay = document.getElementById('loading-overlay');
    if (overlay) {
        overlay.remove();
    }
}

// Keyboard Shortcuts
function initializeKeyboardShortcuts() {
    document.addEventListener('keydown', function(e) {
        // Ctrl/Cmd + shortcuts
        if (e.ctrlKey || e.metaKey) {
            switch(e.key) {
                case 'e':
                    e.preventDefault();
                    window.location.href = '/Student/AvailableExams';
                    break;
                case 's':
                    e.preventDefault();
                    window.location.href = '/Student/MySchedule';
                    break;
                case 'g':
                    e.preventDefault();
                    window.location.href = '/Student/MyGrades';
                    break;
                case 'h':
                    e.preventDefault();
                    window.location.href = '/Student';
                    break;
            }
        }
        
        // ESC to close modals
        if (e.key === 'Escape') {
            const openModal = document.querySelector('.modal.show');
            if (openModal) {
                const modal = bootstrap.Modal.getInstance(openModal);
                modal.hide();
            }
        }
    });
    
    // Show keyboard shortcuts help
    const helpBtn = document.createElement('button');
    helpBtn.className = 'btn btn-outline-secondary btn-sm position-fixed';
    helpBtn.style.cssText = 'bottom: 80px; right: 20px; z-index: 1000;';
    helpBtn.innerHTML = '<i class="fas fa-keyboard"></i>';
    helpBtn.title = 'Phím tắt (?)';
    helpBtn.onclick = showKeyboardShortcuts;
    document.body.appendChild(helpBtn);
}

function showKeyboardShortcuts() {
    const shortcuts = [
        { key: 'Ctrl + E', action: 'Xem bài thi' },
        { key: 'Ctrl + S', action: 'Xem lịch học' },
        { key: 'Ctrl + G', action: 'Xem điểm số' },
        { key: 'Ctrl + H', action: 'Về trang chủ' },
        { key: 'ESC', action: 'Đóng modal' }
    ];
    
    const shortcutsList = shortcuts.map(s => 
        `<tr><td><kbd>${s.key}</kbd></td><td>${s.action}</td></tr>`
    ).join('');
    
    const modalHTML = `
        <div class="modal fade" id="shortcutsModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title"><i class="fas fa-keyboard"></i> Phím tắt</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <table class="table table-sm">
                            <thead>
                                <tr><th>Phím tắt</th><th>Chức năng</th></tr>
                            </thead>
                            <tbody>${shortcutsList}</tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    document.body.insertAdjacentHTML('beforeend', modalHTML);
    const modal = new bootstrap.Modal(document.getElementById('shortcutsModal'));
    modal.show();
    
    document.getElementById('shortcutsModal').addEventListener('hidden.bs.modal', function() {
        this.remove();
    });
}

// Mobile Optimizations
function initializeMobileOptimizations() {
    // Touch gestures for mobile
    if ('ontouchstart' in window) {
        // Add touch feedback
        const touchElements = document.querySelectorAll('.btn, .card, .quick-action-btn');
        touchElements.forEach(element => {
            element.addEventListener('touchstart', function() {
                this.style.transform = 'scale(0.98)';
            });
            
            element.addEventListener('touchend', function() {
                this.style.transform = 'scale(1)';
            });
        });
        
        // Swipe to refresh (simple implementation)
        let startY = 0;
        document.addEventListener('touchstart', function(e) {
            startY = e.touches[0].clientY;
        });
        
        document.addEventListener('touchmove', function(e) {
            const currentY = e.touches[0].clientY;
            const diff = currentY - startY;
            
            if (diff > 100 && window.scrollY === 0) {
                showToast('Kéo xuống để làm mới', 'info', 2000);
            }
        });
    }
    
    // Responsive table handling
    const tables = document.querySelectorAll('.table-responsive');
    tables.forEach(table => {
        if (window.innerWidth < 768) {
            table.style.fontSize = '0.875rem';
        }
    });
}

// Utility Functions
function animateNumber(element, start, end, duration = 1000) {
    const range = end - start;
    const increment = range / (duration / 16);
    let current = start;
    
    const timer = setInterval(() => {
        current += increment;
        if (current >= end) {
            current = end;
            clearInterval(timer);
        }
        element.textContent = Math.floor(current);
    }, 16);
}

function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(() => {
        showToast('Đã sao chép vào clipboard', 'success', 2000);
    }).catch(() => {
        showToast('Không thể sao chép', 'error', 2000);
    });
}

// Auto-save functionality for forms
function initializeAutoSave() {
    const forms = document.querySelectorAll('form[data-autosave]');
    forms.forEach(form => {
        const inputs = form.querySelectorAll('input, textarea, select');
        inputs.forEach(input => {
            input.addEventListener('input', debounce(() => {
                saveFormData(form);
            }, 1000));
        });
    });
}

function saveFormData(form) {
    const formData = new FormData(form);
    const data = Object.fromEntries(formData);
    localStorage.setItem(`form_${form.id}`, JSON.stringify(data));
    showToast('Dữ liệu đã được lưu tự động', 'info', 1000);
}

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Initialize auto-save
initializeAutoSave();

// Export functions for global use
window.StudentUI = {
    showToast,
    showLoadingOverlay,
    hideLoadingOverlay,
    animateNumber,
    copyToClipboard
};
