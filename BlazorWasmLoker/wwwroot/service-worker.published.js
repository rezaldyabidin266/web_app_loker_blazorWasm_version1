// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
/*self.addEventListener('fetch', event => event.respondWith(onFetch(event)));*/
self.addEventListener('fetch', function (event) {

    //Jika dia bukan GET kerjakan secara default
    if (event.request.method != 'GET') return;

    event.respondWith(onFetch(event));
    event.waitUntil(updateFetch(event));

})

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [/\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/];
const offlineAssetsExclude = [/^service-worker\.js$/];

async function onInstall(event) {
    console.info('Service worker: Install');

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash }));
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {

    try {
        let cachedResponse = null;
        let cacheClone = null;

        if (event.request.method === 'GET') {
            // For all navigation requests, try to serve index.html from cache
            // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
            const shouldServeIndexHtml = event.request.mode === 'navigate';
            console.log(event.request.mode);

            const request = shouldServeIndexHtml ? 'index.html' : event.request;
            const cache = await caches.open(cacheName);
            cachedResponse = await cache.match(request);
            //if (cachedResponse) return cachedResponse

            //const networkResponse = await fetch(event.request);
            //event.waitUntil(
            //    cache.put(event.request, networkResponse.clone())
            //);
            //return networkResponse;
        }

        cacheClone = await caches.open(cacheName).then((cache) => {
            //dicocok kan urlnya
            return cache.match(event.request).then((response) => {
                //bila cocok pake response bila gak cocok request lagi
                return response || fetch(event.request).then((response) => {
                    //console.log("ini RESPONSE",response)
                    cache.put(event.request, response.clone());
                    return response;
                }, (error) => {
                    console.warn(error);
                    throw error;
                })
            })
        })

        let jaringan = await fetch(event.request)

        return cachedResponse || jaringan;

    } catch (e) {

        let cachedResponse = null;
        let cacheClone = null;

        if (event.request.method === 'GET') {
            // For all navigation requests, try to serve index.html from cache
            // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
            const shouldServeIndexHtml = event.request.mode === 'navigate';
            console.log(event.request.mode);

            const request = shouldServeIndexHtml ? 'index.html' : event.request;
            const cache = await caches.open(cacheName);
            cachedResponse = await cache.match(request);
        }

        cacheClone = await caches.open(cacheName).then((cache) => {
            //dicocok kan urlnya
            return cache.match(event.request).then((response) => {
                //bila cocok pake response bila gak cocok request lagi
                return response || fetch(event.request).then((response) => {
                    //console.log("ini RESPONSE",response)
                    cache.put(event.request, response.clone());
                    return response;
                }, (error) => {
                    console.warn(error);
                    throw error;
                })
            })
        })

        return  cachedResponse || cacheClone;
    }


    //cacheClone = await caches.open(cacheName).then((cache) => {
    //    //dicocok kan urlnya
    //    return cache.match(event.request).then((response) => {
    //        //bila cocok pake response bila gak cocok request lagi
    //        return response || fetch(event.request).then((response) => {
    //            //console.log("ini RESPONSE",response)
    //            cache.put(event.request, response.clone());
    //            return response;
    //        }, (error) => {
    //            console.warn(error);
    //            throw error;
    //        })
    //    })
    //})

    //return cachedResponse || cacheClone || fetch(event.request);
}

function updateFetch(event) {
    return caches.open(cacheName).then((cache) => {
        return fetch(event.request).then((response) => {
            return cache.put(event.request, response);
        })
    })
}