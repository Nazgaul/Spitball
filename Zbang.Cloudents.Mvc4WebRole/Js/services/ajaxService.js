mItem.factory('ajaxService',
    ['$http',
     '$q',
     '$angularCacheFactory',
     '$analytics',

    function ($http, $q, $angularCacheFactory, $analytics) {
        var ttls = {},
        service = {
            get: function (url, data, ttl) {
                var dfd = $q.defer(),
                    startTime = new Date().getTime();

                ttl = ttl || 600000; //default to 10 mins

                var cache = getCache(ttl);

                $http.get(url, { params: data, cache: cache }).success(function (response) {
                    dfd.resolve(response);
                    trackTime(startTime, url, data);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;

            },
            post: function (url, data) {
                var dfd = $q.defer(),
                startTime = new Date().getTime();

                $http.post(url, data).success(function (response) {
                    dfd.resolve(response);
                    trackTime(startTime, url, data);
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

            var cache = $angularCacheFactory.get(ttlString);

            if (cache) {
                ttls[ttlString] = cache;
                return cache;
            }

            cache = $angularCacheFactory(ttlString, {
                maxAge: ttl
            });

            ttls[ttlString] = cache;

            return cache;
        }

    }]);
