app.factory('ajaxService',
    ['$http',
     '$q',
     '$angularCacheFactory',
     '$analytics',

    function ($http, $q, $angularCacheFactory, $analytics) {
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
                    ttl = ttl || 60000; //default to 1 mins
                    getObj.cache = getCache(ttl);
                }
                $http.get(url, getObj).success(function (response) {
                    trackTime(startTime, url, data);
                    if (response.success) {
                        dfd.resolve(response.payload);
                        return;
                    }
                    dfd.reject(response.payload);

                }).error(function (response) {
                    dfd.reject(response);
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

                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            }
        }

        return service;

        function trackTime(startTime, url, data) {
            var timeSpent = new Date().getTime() - startTime + 'ms';

            var properties = {
                category: url.toLowerCase() !== '/item/preview/' ? 'ajax' : 'ajaxPreview',
                timeSpent: timeSpent,
                value: url,
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
