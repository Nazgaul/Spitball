app.factory('ajaxService',
    ['$http',
     '$q',
     '$angularCacheFactory',
     '$analytics',
     '$log',

    function ($http, $q, $angularCacheFactory, $analytics, $log) {
        "use strict";
        var ttls = {},
        service = {
            get: function (url, data, ttl, noCache) {
                var dfd = $q.defer(),
                    startTime = new Date().getTime();

                var getObj = {
                    params: data
                };
                if (!noCache) {
                    ttl = ttl || 15000; //default to 30 seconds
                    getObj.cache = getCache(ttl);
                }
                $http.get(url, getObj).success(function (response) {
                    trackTime(startTime, url, data);
                    if (response.success) {
                        dfd.resolve(response.payload);
                        return;
                    }
                    dfd.reject(response.payload);
                    logError(url, data, response.payload);

                }).error(function (response) {
                    dfd.reject(response);
                    logError(url, data, response);
                });
                return dfd.promise;

            },
            post: function (url, data, disableClearCache) {
                var dfd = $q.defer(),
                startTime = new Date().getTime();
                $http.post(url, data).success(function (response) {
                    trackTime(startTime, url, data);
                    if (!disableClearCache) {
                        $angularCacheFactory.clearAll();
                    }

                    if (response.success) {
                        dfd.resolve(response.payload);
                        return;
                    }

                    dfd.reject(response.payload);
                    logError(url, data, response);

                }).error(function (response) {
                    dfd.reject(response);
                    logError(url, data, response);
                });
                return dfd.promise;
            }
        }

        return service;

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
                    errorMessage: log,
                    cause: 'ajaxRequest',
                    stackTrace:''
                })
            });
        }
        function trackTime(startTime, url, data) {
            var timeSpent = new Date().getTime() - startTime;

            var properties = {
                category: url.toLowerCase() !== '/item/preview/' ? 'ajax' : 'ajaxPreview',
                timeSpent: timeSpent,
                variable: url,
                label: JSON.stringify(data)
            };

            $analytics.timingTrack(properties);
        }


        function getCache(ttl) {
            var ttlString = ttl.toString();
            if (ttls[ttlString]) {
                return ttls[ttlString];
            }

            var cache = $angularCacheFactory(ttlString, {
                maxAge: ttl
            });

            ttls[ttlString] = cache;

            return cache;
        }

    }]);
