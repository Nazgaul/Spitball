(function () {
    angular.module('app.ajaxservice').factory('ajaxService', ajaxService);
    ajaxService.$inject = ['$http', '$q', 'Analytics', 'CacheFactory'];



    function ajaxService($http, $q, analytics, cacheFactory) {
        "use strict";
        var cancelObjs = {};

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
                    logError(url, data);
                    dfd.reject();
                    return;
                }
                if (retVal.success) {
                    dfd.resolve(retVal.payload);
                    return;
                }

                dfd.reject(retVal.payload);
                //logError(url, data, retVal);

            }, function (response) {
                dfd.reject(response);
                logError(url, data, response);
            });
            return dfd.promise;
        }


        function get(url, data, ttl) {
            var dfd = $q.defer(),
                startTime = new Date().getTime();

            var getObj = {
                params: data
            };


            if (cancelObjs[url]) {
                cancelObjs[url].resolve();
            }

            cancelObjs[url] = $q.defer();
            getObj.timeout = cancelObjs[url].promise;

            ttl = ttl || 45000;
            getObj.cache = getCache(ttl);


            $http.get(buildUrl(url), getObj).then(function (response) {
                //var data = response.data;
                var retVal = response.data;
                trackTime(startTime, url, data, 'get');

                if (!retVal) {
                    dfd.reject();
                    return;
                }

                if (retVal.success) {
                    dfd.resolve(retVal.payload);
                    cancelObjs[url] = null;
                    return;
                }
                dfd.reject(retVal.payload);
                cancelObjs[url] = null;

            }, function (response) {
                dfd.reject(response);
                logError(url, data, response);
            });
            return dfd.promise;

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


        function getCache(ttl) {
            var ttlString = ttl.toString();

            var dataCache = cacheFactory.get(ttlString);
            if (!dataCache) {
                dataCache = cacheFactory(ttlString, {
                    maxAge: ttl
                });
            }
            return dataCache;
        }

        return {
            get: get,
            post: post
        };

    }
})();