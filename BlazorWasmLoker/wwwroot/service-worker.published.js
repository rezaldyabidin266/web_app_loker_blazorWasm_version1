// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', function (event) {

    event.waitUntil(onInstall(event));

    checkNetworkState();

});
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));

let clientW;

self.addEventListener('fetch', function (event) {

    /*    clientW = clients.get(event);*/

    //USER OFFFLINE
    if (!navigator.onLine) {
        console.log('INI OFFLINE')
        if (event.request.method === "POST") {
            const shouldServeIndexHtml = event.request.mode === 'navigate' && !event.request.url.includes('/Identity/');
            const request = shouldServeIndexHtml ? 'index.html' : event.request;

            var authHeader = event.request.headers.get('token');


            console.log('INI POST OFFLINE');

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

function checkNetworkState() {
    setInterval(function () {
        if (navigator.onLine) {
            sendOfflinePostRequestsToServer();

        }
    }, 3000);

    const bct = new BroadcastChannel('install-channel');

    bct.onmessage = (event) => {
        console.log(event)
        bct.postMessage({ token: 'MASIH KOSONG Install' });
    };
}

async function sendOfflinePostRequestsToServer() {
    var request = indexedDB.open("PostData");
    const broadcast = new BroadcastChannel('count-channel');
    const broadcastToken = new BroadcastChannel('token-channel');
    var localRespon;

    broadcast.onmessage = (event) => {
        localRespon = event.data;
       // console.log(localRespon);
    };

    broadcastToken.onmessage = (event) => {
        console.log(event)
        broadcastToken.postMessage({ token : 'MASIH KOSONG'});
    };

    request.onsuccess = function (event) {
        var db = event.target.result;
        var tx = db.transaction('postObject', 'readwrite');
        var store = tx.objectStore('postObject');
        var allRecords = store.getAll();
        allRecords.onsuccess = function () {

            if (allRecords.result && allRecords.result.length > 0) {
                //console.warn(localRespon);//undefined

                var records = allRecords.result
                //make recursive call to hit fetch requests to server in a serial manner
                var resp = sendFetchRequestsToServer(
                    fetch(records[0].url, {
                        method: "post",
                        headers: {
                            'Accept': 'application/json',
                            'Content-Type': 'application/json',
                            'Authorization': records[0].authHeader,
                      
                        },
                        body: records[0].payload
                    }).then(response => response.json()).then((res) => {

                        //console.log(res.token);
                        broadcast.postMessage({ token: res.token });

                    }), records[0].url, records[0].authHeader, records[0].payload, records.slice(1))

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
        if (!db.objectStoreNames.contains('postObject')) {
            objectStore = db.createObjectStore("postObject", { keyPath: 'id', autoIncrement: true });
            objectStore.createIndex("url", "url", { unique: false });
        }
        else {
            objectStore = db.objectStoreNames.get('postObject');
        }
    }
}

function saveIntoIndexedDb(url, authHeader, payload) {

    var jsonPayLoad = JSON.parse(payload)
    var myRequest = {
        url: url.url,
        authHeader: authHeader,
        payload: JSON.stringify(jsonPayLoad)
    };

    var request = indexedDB.open("PostData");

    request.onsuccess = function (event) {
        var db = event.target.result;
        var tx = db.transaction('postObject', 'readwrite');
        var store = tx.objectStore('postObject');

        store.add(myRequest);
        //for (var i in myRequest) {
        //    store.add(myRequest[i]);
        //}
    }
}


async function sendFetchRequestsToServer(data, reqUrl, authHeader, payload, records) {


    let promise = Promise.resolve(data).then((response) => {

        console.log('Successfully sent request to server');
        if (records.length != 0) {

            sendFetchRequestsToServer(
                fetch(records[0].url, {
                    method: "post",
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json',
                        'Authorization': records[0].authHeader,
                
                    },
                    body: records[0].payload
                }).then((response) => { console.log(response.json()) }), records[0].url, records[0].authHeader, records[0].payload, records.slice(1))
        }
        return true
    }).catch((e) => {
        //fetch fails only in case of network error. Fetch is successful in case of any response code
        console.log('Exception while sending post request to server' + e)
        saveIntoIndexedDb(reqUrl, authHeader, payload)
    })
}


