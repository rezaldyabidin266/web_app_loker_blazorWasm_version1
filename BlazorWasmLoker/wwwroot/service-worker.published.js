// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
/*self.addEventListener('fetch', event => event.respondWith(onFetch(event)));*/
self.addEventListener('fetch', function (event) {


    //USER OFFFLINE
    if (!navigator.onLine) {
        console.log('INI OFFLINE')
        if (event.request.method === "POST") {
            const shouldServeIndexHtml = event.request.mode === 'navigate' && !event.request.url.includes('/Identity/');
            const request = shouldServeIndexHtml ? 'index.html' : event.request;
            var authHeader = event.request.headers
            console.log('INI POST OFFLINE');
            console.log(event.request)
            Promise.resolve(event.request.text()).then((payload) => {
                //save offline requests to indexed db
                saveIntoIndexedDb(request, authHeader, payload)
            })
        }

    }

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
        if (event.request.method === 'GET') {
            // For all navigation requests, try to serve index.html from cache
            // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
            const shouldServeIndexHtml = event.request.mode === 'navigate' && !event.request.url.includes('/Identity/');

            const request = shouldServeIndexHtml ? 'index.html' : event.request;
            const cache = await caches.open(cacheName);
            cachedResponse = await fetch(request).catch(() => {
                return cache.match(request)
            });

            cacheClone = await caches.open(cacheName).then((cache) => {
                //dicocok kan urlnya
                return cache.match(event.request).then((response) => {
                    //bila cocok pake response bila gak cocok request lagi
                    return response || fetch(request).then((response) => {
                        //console.log("ini RESPONSE",response)
                        cache.put(event.request, response.clone());
                        return response;
                    }, (error) => {
                        console.warn(error);
                        throw error;
                    })
                })
            })
        } else {
            if (!navigator.onLine) {
                const shouldServeIndexHtml = event.request.mode === 'navigate' && !event.request.url.includes('/Identity/');
                const request = shouldServeIndexHtml ? 'index.html' : event.request;
                console.log(request.method);
                console.log(request.formData());
                console.log(request.url);
                console.log('INI POST OFFLINE');
                var authHeader = event.request.headers
                Promise.resolve(request.text()).then((payload) => {
                    //save offline requests to indexed db
                    saveIntoIndexedDb(request, authHeader, payload)
                })

            }
        }

        return cachedResponse;

    } catch (e) {

        let cachedResponse = null;
        if (event.request.method === 'GET') {
            // For all navigation requests, try to serve index.html from cache
            // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
            const shouldServeIndexHtml = event.request.mode === 'navigate' && !event.request.url.includes('/Identity/');

            const request = shouldServeIndexHtml ? 'index.html' : event.request;
            const cache = await caches.open(cacheName);
            cachedResponse = await cache.match(request);
            cacheClone = await caches.open(cacheName).then((cache) => {
                //dicocok kan urlnya
                return cache.match(event.request).then((response) => {
                    //bila cocok pake response bila gak cocok request lagi
                    return response || fetch(request).then((response) => {
                        //console.log("ini RESPONSE",response)
                        cache.put(event.request, response.clone());
                        return response;
                    }, (error) => {
                        console.warn(error);
                        throw error;
                    })
                })
            })
        } else {
            if (!navigator.onLine) {
                const shouldServeIndexHtml = event.request.mode === 'navigate' && !event.request.url.includes('/Identity/');
                const request = shouldServeIndexHtml ? 'index.html' : event.request;

                console.log(request.formData());
                console.log(request.url);
                console.log(request.method);
                console.log('Catch INI POST OFFLINE');

                var authHeader = event.request.headers
                Promise.resolve(request.text()).then((payload) => {
                    //save offline requests to indexed db
                    saveIntoIndexedDb(request, authHeader, payload)
                })
            }
        }

        return cachedResponse;
    }
}

function updateFetch(event) {
    const shouldServeIndexHtml = event.request.mode === 'navigate' && !event.request.url.includes('/Identity/');
    const request = shouldServeIndexHtml ? 'index.html' : event.request;
    return caches.open(cacheName).then((cache) => {
        return fetch(request).then((response) => {
            return cache.put(event.request, response);
        })
    })
}

function saveIntoIndexedDb(url, authHeader, payload) {
    var myRequest = {};
    /*jsonPayLoad = JSON.parse(payload);*/
    console.log(payload)
    jsonPayLoad = payload;
    myRequest.url = url;
    myRequest.authHeader = authHeader;
    /*myRequest.payload = JSON.stringify(jsonPayLoad);*/
    myRequest.payload = payload;
    var databaseOpen = indexedDB.open("DatabaseIndex",1);
    console.log(myRequest)
    databaseOpen.onsuccess = function (event) {
        var db = event.target.result;
        var tx = db.transaction(["postrequest"], 'readwrite');
        tx.objectStore("postrequest").add(myRequest);
/*        store.add(myRequest)*/
    }
}
