// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', function (event) {

    event.waitUntil(onInstall(event));

    checkNetworkState();

});
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));

self.addEventListener('fetch', function (event) {

    //USER OFFFLINE
    if (!navigator.onLine) {

        if (event.request.method != 'GET') {
            const shouldServeIndexHtml = event.request.mode === 'navigate' && !event.request.url.includes('/Identity/');
            const request = shouldServeIndexHtml ? 'index.html' : event.request;

            var authHeader = event.request.headers.get("Content-Type");
            console.log('INI POST OFFLINE');

            if ((request.url.indexOf('/upload-foto') !== -1) || (request.url.indexOf('/upload-cv') !== -1)) {

                Promise.resolve(event.request.formData()).then((payload) => {
                    //save offline requests to indexed db
                    saveIntoIndexedDb(request, authHeader, payload)
                })

            }
            else {
                Promise.resolve(event.request.text()).then((payload) => {
                    //save offline requests to indexed db
                    saveIntoIndexedDb(request, authHeader, payload)
                })
            }
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

var LocalStorage;

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

    event.waitUntil(self.clients.claim());

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

                        throw error;
                    })
                })
            })

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
                        throw error;
                    })
                })
            })

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

self.addEventListener('message', (event) => {
    LocalStorage = event.data;
});

function checkNetworkState() {

    setInterval(function () {
        if (navigator.onLine) {
            sendOfflinePostRequestsToServer(LocalStorage);
        }
    }, 3000);

}

async function sendOfflinePostRequestsToServer(LocalStorage) {
    var request = indexedDB.open("PostData");

    //BroadcastaChannel
    const broadcast = new BroadcastChannel('count-channel');
    broadcast.onmessage = (event) => { };

    request.onsuccess = function (event) {
        var db = event.target.result;
        var tx = db.transaction('postObject', 'readwrite');
        var store = tx.objectStore('postObject');
        var allRecords = store.getAll();
        allRecords.onsuccess = function () {

            if (allRecords.result && allRecords.result.length > 0) {

                var arrayIdPengalaman;
                if (LocalStorage.pengalamanId === null) {
                    arrayIdPengalaman = ['Array Kosong'];
                } else {
                    arrayIdPengalaman = LocalStorage.pengalamanId
                }

                var jsonToken = JSON.parse(LocalStorage.token)
                var records = allRecords.result

                for (var i = 0; i < allRecords.result.length; i++) {

                    if ((records[i].url.indexOf('/upload-foto') !== -1) || (records[i].url.indexOf('/upload-cv') !== -1)) {

                        var form_data = new FormData();
                        var itemFile = records[i].payload;

                        for (var key in itemFile) {
                            console.log(key);
                            console.log(itemFile[key]);
                            form_data.append(key, itemFile[key]);
                        }

                        //make recursive call to hit fetch requests to server in a serial manner
                        var resp = sendFetchRequestsToServer(
                            fetch(records[i].url, {
                                method: records[i].method,
                                headers: {
                                    'token': jsonToken,
                                },
                                body: form_data
                            }).then(response => response.json()).then((res) => {

                                broadcast.postMessage({ pesan: 'Berhasil request data offline, diharapkan segera online' });

                            }).catch((error) => { console.log(error) }), records[i].url, records[i].authHeader, records[i].payload, records.slice(1), LocalStorage)
                    }
                    else {

                        //make recursive call to hit fetch requests to server in a serial manner
                        var resp = sendFetchRequestsToServer(
                            fetch(records[i].url, {
                                method: records[i].method,
                                headers: {
                                    'Accept': 'application/json',
                                    'Content-Type': 'application/json',
                                    'token': jsonToken,
                                    'lokerId': LocalStorage.idLoker,
                                    'pengalamanId': arrayIdPengalaman[i]
                                },
                                body: records[i].payload
                            }).then(response => response.json()).then((res) => {

                                broadcast.postMessage({ pesan: 'Berhasil request data offline, diharapkan segera online' });

                            }).catch((error) => { console.log(error) }), records[i].url, records[i].authHeader, records[i].payload, records.slice(1), LocalStorage)

                    }
                }

                for (var i = 0; i < allRecords.result.length; i++)
                    store.delete(allRecords.result[i].id)
            }
        };
    }
    request.onupgradeneeded = function (event) {
        var db = event.target.result;
        db.onerror = function (event) {
            console.log("Why didn't you allow my web app to use IndexedDB?!");
        };

        var objectStore;
        if (!db.objectStoreNames.contains('postObject')) {     //jangan pernah hapus keyPath!
            objectStore = db.createObjectStore("postObject", { keyPath: 'id', autoIncrement: true });
            objectStore.createIndex("url", "url", { unique: false });
        }
        else {
            objectStore = db.objectStoreNames.get('postObject');
        }
    }
}

function saveIntoIndexedDb(url, authHeader, payload) {

    var myRequest;

    //JIKA DIA DELETE
    if (url.method === 'DELETE') {

        myRequest = {
            url: url.url,
            authHeader: authHeader,
            payload: JSON.stringify(payload),
            method: url.method
        };

    } else {

            if ((url.url.indexOf('/upload-foto') !== -1) || (url.url.indexOf('/upload-cv') !== -1)) {

                var formDataFile = {};
                payload.forEach((value, key) => formDataFile[key] = value);

                myRequest = {
                    url: url.url,
                    authHeader: authHeader,
                    payload: formDataFile,
                    method: url.method,
                };

            } else {

                var jsonPayLoad = JSON.parse(payload)
                myRequest = {
                    url: url.url,
                    authHeader: authHeader,
                    payload: JSON.stringify(jsonPayLoad),
                    method: url.method,
                };

            }


    }


    var request = indexedDB.open("PostData");
    request.onsuccess = function (event) {
        var db = event.target.result;
        var tx = db.transaction('postObject', 'readwrite');
        var store = tx.objectStore('postObject');

        store.add(myRequest);

        //get IndexDb Store
        var allRecords = store.getAll();
        console.log(allRecords);
    }
}


async function sendFetchRequestsToServer(data, reqUrl, authHeader, payload, records, LocalStorage) {


    let promise = Promise.resolve(data).then((response) => {

        console.log('Successfully sent request to server');

        return true
    }).catch((e) => {
        //fetch fails only in case of network error. Fetch is successful in case of any response code
        console.log('Exception while sending post request to server' + e)
        saveIntoIndexedDb(reqUrl, authHeader, payload)
    })
}


