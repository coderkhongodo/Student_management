// Service Worker for Student Management System
// Version 1.0.0

const CACHE_NAME = 'student-management-v1';
const urlsToCache = [
    '/',
    '/css/design-system.css',
    '/css/cross-browser-fixes.css',
    '/css/mobile-optimizations.css',
    '/css/site.css',
    '/css/admin-ui.css',
    '/css/teacher-ui.css',
    '/css/student-ui.css',
    '/lib/bootstrap/dist/css/bootstrap.min.css',
    '/lib/bootstrap/dist/js/bootstrap.bundle.min.js',
    '/lib/jquery/dist/jquery.min.js',
    '/js/site.js',
    'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css',
    'https://cdnjs.cloudflare.com/ajax/libs/animate.css/4.1.1/animate.min.css'
];

// Install event - cache resources
self.addEventListener('install', function(event) {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(function(cache) {
                console.log('Opened cache');
                return cache.addAll(urlsToCache);
            })
            .catch(function(error) {
                console.log('Cache install failed:', error);
            })
    );
});

// Fetch event - serve from cache, fallback to network
self.addEventListener('fetch', function(event) {
    event.respondWith(
        caches.match(event.request)
            .then(function(response) {
                // Cache hit - return response
                if (response) {
                    return response;
                }

                return fetch(event.request).then(
                    function(response) {
                        // Check if we received a valid response
                        if (!response || response.status !== 200 || response.type !== 'basic') {
                            return response;
                        }

                        // Clone the response
                        var responseToCache = response.clone();

                        caches.open(CACHE_NAME)
                            .then(function(cache) {
                                cache.put(event.request, responseToCache);
                            });

                        return response;
                    }
                );
            })
            .catch(function() {
                // Return offline page for navigation requests
                if (event.request.destination === 'document') {
                    return caches.match('/offline.html');
                }
            })
    );
});

// Activate event - clean up old caches
self.addEventListener('activate', function(event) {
    var cacheWhitelist = [CACHE_NAME];

    event.waitUntil(
        caches.keys().then(function(cacheNames) {
            return Promise.all(
                cacheNames.map(function(cacheName) {
                    if (cacheWhitelist.indexOf(cacheName) === -1) {
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
});

// Background sync for offline form submissions
self.addEventListener('sync', function(event) {
    if (event.tag === 'background-sync') {
        event.waitUntil(doBackgroundSync());
    }
});

function doBackgroundSync() {
    // Handle offline form submissions when back online
    return new Promise(function(resolve, reject) {
        // Implementation would depend on specific offline requirements
        resolve();
    });
}

// Push notifications (for future implementation)
self.addEventListener('push', function(event) {
    const options = {
        body: event.data ? event.data.text() : 'New notification',
        icon: '/icon-192x192.png',
        badge: '/badge-72x72.png',
        vibrate: [100, 50, 100],
        data: {
            dateOfArrival: Date.now(),
            primaryKey: 1
        },
        actions: [
            {
                action: 'explore',
                title: 'View',
                icon: '/icon-explore.png'
            },
            {
                action: 'close',
                title: 'Close',
                icon: '/icon-close.png'
            }
        ]
    };

    event.waitUntil(
        self.registration.showNotification('Student Management System', options)
    );
});

// Handle notification clicks
self.addEventListener('notificationclick', function(event) {
    event.notification.close();

    if (event.action === 'explore') {
        // Open the app
        event.waitUntil(
            clients.openWindow('/')
        );
    } else if (event.action === 'close') {
        // Just close the notification
        event.notification.close();
    } else {
        // Default action
        event.waitUntil(
            clients.openWindow('/')
        );
    }
});

// Message handling for cache updates
self.addEventListener('message', function(event) {
    if (event.data && event.data.type === 'SKIP_WAITING') {
        self.skipWaiting();
    }
    
    if (event.data && event.data.type === 'UPDATE_CACHE') {
        event.waitUntil(
            caches.open(CACHE_NAME).then(function(cache) {
                return cache.addAll(event.data.urls);
            })
        );
    }
});

// Performance monitoring
self.addEventListener('fetch', function(event) {
    const startTime = performance.now();
    
    event.respondWith(
        caches.match(event.request).then(function(response) {
            const endTime = performance.now();
            const duration = endTime - startTime;
            
            // Log slow requests for monitoring
            if (duration > 1000) {
                console.warn(`Slow request: ${event.request.url} took ${duration}ms`);
            }
            
            return response || fetch(event.request);
        })
    );
});

// Error handling
self.addEventListener('error', function(event) {
    console.error('Service Worker error:', event.error);
});

self.addEventListener('unhandledrejection', function(event) {
    console.error('Service Worker unhandled promise rejection:', event.reason);
});

console.log('Service Worker loaded successfully');
