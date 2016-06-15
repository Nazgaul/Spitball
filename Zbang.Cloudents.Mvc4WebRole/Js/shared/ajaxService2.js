'use strict';
(function () {
    angular.module('app.ajaxservice').factory('ajaxService2', ajaxService);
    ajaxService.$inject = ['$http', '$q', 'Analytics', 'CacheFactory', 'routerHelper'];


    function ajaxService($http, $q, analytics, cacheFactory, routerHelper) {
        var cancelObjs = {};
        var cacheCategories = {
            'dashboard': { 'ttl': 1800000 },
            'settings': { 'ttl': 1800000 }
        };
        function post(url, data, disableClearCache) {
            var dfd = $q.defer(),
                startTime = new Date().getTime();

            $http.post(buildUrl(url), data).then(function (response) {
                var retVal = response.data;
                trackTime(startTime, url, data, 'post');
                if (!disableClearCache) {
                    cacheFactory.clearAll();
                    //angular.forEach(ttls, function (ttl) {
                    //    ttl.removeAll();
                    //});
                }
                if (!retVal) {
                    logError(url, data, retVal);
                    dfd.reject();
                    return;
                }
                if (retVal.success) {
                    dfd.resolve(retVal.payload);
                    return;
                }

                dfd.reject(retVal.payload);

            }, function (response) {
                dfd.reject(response);
                logError(url, data, response);
            });
            return dfd.promise;
        }

        function getHtml(url) {
            var dfd = $q.defer(),
                startTime = new Date().getTime();
            if (cancelObjs[url]) {
                cancelObjs[url].resolve();
            }

            cancelObjs[url] = $q.defer();
            var getObj = {};
            getObj.timeout = cancelObjs[url].promise;
            url = buildUrl(url);
            url = routerHelper.buildUrl(url);
            $http.get(url, getObj).then(function (response) {
                trackTime(startTime, url, 'get html');
                var data = response.data;
                if (!data) {
                    dfd.reject();
                    return;
                }
                cancelObjs[url] = null;
                dfd.resolve(data);
                return;

            }, function (response) {
                dfd.reject(response);
                logError(url, response);
            });
            return dfd.promise;
        }
        function get(url, data, category, disableCancel) {
            var deferred = $q.defer();
            if (category) {
                var categoryFactory = cacheFactory.get(category);

                if (categoryFactory) {    // if there is factory fot the current cateegory - bring the cache
                    deferred.resolve(categoryFactory);
                }
                else {            // first time call with cacheInfo to be saved
                    getFromServer(category);
                }
            }
            else { // no need in cache
                getFromServer();
            }

            return deferred.promise;


            function getFromServer(cacheCategory) {
                var startTime = new Date().getTime();
                $http.get(buildUrl(url)).then(function (response) {
                    //var data = response.data;
                    var retVal = response.data;
                    trackTime(startTime, url, data, 'get');

                    if (!retVal) {
                        deferred.reject();
                        return;
                    }
                    cancelObjs[url] = null;
                    if (retVal.success) {
                        if (cacheCategory) {
                            var categoryFactory = cacheFactory(cacheCategory, {
                                deleteOnExpire: 'aggressive',
                                maxAge: cacheCategories[cacheCategory].ttl,
                                recycleFreq: 15000, // 15 seconds
                                storageMode: 'localStorage',
                                storagePrefix: 'sb.c.'
                            });

                            categoryFactory.put("payload", retVal.payload);
                        }
                        deferred.resolve(retVal.payload);
                        return;
                    }
                    deferred.reject(retVal.payload);
                }, function (response) {
                    dfd.reject(response);
                    logError(url, data, response);
                });
            }

        }
        function buildUrl(url) {
            url = url.toLowerCase();
            if (!url.startsWith('/')) {
                url = '/' + url;
            }
            if (!url.endsWith('/')) {
                url = url + '/';
            };
            return url;
        }


        function logError(url, data, payload) {
            var log = {
                data: data,
                payload: payload
            };

            $.ajax({
                type: 'POST',
                url: '/error/jslog/',
                contentType: 'application/json',
                data: angular.toJson({
                    errorUrl: url,
                    errorMessage: JSON.stringify(log),
                    cause: 'ajaxRequest',
                    stackTrace: ''
                })
            });
        }

        function trackTime(startTime, url, data, type) {
            var timeSpent = new Date().getTime() - startTime;

            analytics.trackTimings(url.toLowerCase() !== '/item/preview/' ? 'ajax ' + type
                    : 'ajaxPreview', url, timeSpent, JSON.stringify(data));
            //$analytics.timingTrack(properties);
        }

        return {
            get: get,
            post: post,
            getHtml: getHtml
        };

    }
})();