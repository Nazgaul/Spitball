(function  (){
    angular.module('app.ajaxservice').factory('ajaxService', ajaxService);
    ajaxService.$inject = [
        '$http',
        '$q',
        '$angularCacheFactory',
        '$analytics'
    ];

//'$log',

    function ajaxService($http, $q, $angularCacheFactory, $analytics /*, $log*/) {
        "use strict";
        var ttls = {},
            cancelObjs = {},
            service = {
                get: function(url, data, ttl, noCache, cancellable) {
                    var dfd = $q.defer(),
                        startTime = new Date().getTime();

                    var getObj = {
                        params: data
                    };

                    if (cancelObjs[url]) {
                        cancelObjs[url].resolve();
                    }

                    if (cancellable) {
                        cancelObjs[url] = $q.defer();
                        getObj.timeout = cancelObjs[url].promise;
                    }

                    if (!noCache) {
                        ttl = ttl || 15000; //default to 30 seconds
                        getObj.cache = getCache(ttl);
                    }
                    url = url.toLowerCase();
                    $http.get(url, getObj).success(function(response) {
                        trackTime(startTime, url, data, 'get');

                        if (!response) {
                            dfd.reject();
                            return;
                        }

                        if (response.success) {
                            dfd.resolve(response.payload);
                            cancelObjs[url] = null;
                            return;
                        }
                        dfd.reject(response.payload);
                        cancelObjs[url] = null;
                        logError(url, data, response);

                    }).error(function(response) {
                        dfd.reject(response);
                        logError(url, data, response);
                    });
                    return dfd.promise;

                },
                post: function(url, data, disableClearCache) {
                    var dfd = $q.defer(),
                        startTime = new Date().getTime();
                    url = url.toLowerCase();
                    $http.post(url, data).success(function(response) {
                        trackTime(startTime, url, data, 'post');
                        if (!disableClearCache) {
                            angular.forEach(ttls, function(ttl) {
                                ttl.removeAll();
                            });
                        }
                        if (!response) {
                            logError(url, data);
                            dfd.reject();
                            return;
                        }
                        if (response.success) {
                            dfd.resolve(response.payload);
                            return;
                        }

                        dfd.reject(response.payload);
                        logError(url, data, response);

                    }).error(function(response) {
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
                    errorMessage: JSON.stringify(log),
                    cause: 'ajaxRequest',
                    stackTrace: ''
                })
            });
        }

        function trackTime(startTime, url, data, type) {
            var timeSpent = new Date().getTime() - startTime;

            var properties = {
                category: url.toLowerCase() !== '/item/preview/' ? 'ajax ' + type
                    : 'ajaxPreview',
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

    }
})();