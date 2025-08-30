self.addEventListener('push', event => {
  const data = event.data ? event.data.json() : {};
  const title = data.title || 'New message';
  const options = {
    body: data.body || '',
    icon: 'https://home.jokora.dev/favicon-192x192.png',
    badge: 'https://home.jokora.dev/favicon-64x64.png',
    renotify: true,
    tag: data.tag,
    vibrate: [200, 100, 300],
    data
  };
  event.waitUntil(self.registration.showNotification(title, options));
});

self.addEventListener('notificationclick', event => {
  event.notification.close();
  const url = (event.notification.data && event.notification.data.url) || '/';
  event.waitUntil(clients.openWindow(url));
});
