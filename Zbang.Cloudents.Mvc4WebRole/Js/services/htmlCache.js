app.factory('htmlCache',
    ['sUserDetails', '$angularCacheFactory', 'sVerChecker', '$http', '$window', '$templateCache',
    function (sUserDetails, $angularCacheFactory, sVerChecker, $http, $window, $templateCache) {
        "use strict";

        var htmlCache = $angularCacheFactory('htmlCache', {
            maxAge: 2592000000,
            deleteOnExpire: 'aggressive',
            storageMode: 'localStorage'
        });

        var pages = {
            dashboard: '/dashboard/indexpartial/',
            box: '/box/indexpartial/',
            item: '/item/indexpartial/',
            quiz: '/quiz/indexpartial/',
            library: '/library/indexpartial/'
        };


        var service = {
            cachePages: cachePages,
            checkState: checkState
        };

        return service;

        function cachePages() {
            

            if (sUserDetails.isAuthenticated()) {
                $http.get(pages.dashboard, { cache: htmlCache }).then(function (res) { putInCache(pages.dashboard, res); });
                $http.get(pages.library, { cache: htmlCache }).then(function (res) { putInCache(pages.library, res); });
            }

            $http.get(pages.box, { cache: htmlCache }).then(function (res) { putInCache(pages.box, res); });
            $http.get(pages.item, { cache: htmlCache }).then(function (res) { putInCache(pages.item, res); });
            $http.get(pages.quiz, { cache: htmlCache }).then(function (res) { putInCache(pages.quiz, res); });
            

            htmlCache.put('version', sVerChecker.currentVersion());
            htmlCache.put('culture', sUserDetails.getDetails().culture);            

            function putInCache(key, res) {
                $templateCache.put(key, res.data);
            }
        }

        function checkState() {
            console.log(sUserDetails.getDetails().culture);

            if (!htmlCache.keys().length) {
                service.cachePages();
                return;
            }
            if ((htmlCache.get('version') !== sVerChecker.currentVersion()) || (htmlCache.get('culture') !== sUserDetails.getDetails().culture)) {
                htmlCache.removeAll();
                service.cachePages();
            }
        }

    }
    ]);







