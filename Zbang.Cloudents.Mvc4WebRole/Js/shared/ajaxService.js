(function () {
    'use strict';
    angular.module('app.ajaxservice').factory('ajaxService', ajaxService);
    ajaxService.$inject = ['$http', '$q', 'Analytics' ];



    function ajaxService($http, $q, analytics) {
        var cancelObjs = {};

        function post(url, data) {
            var dfd = $q.defer(),
                startTime = new Date().getTime();

            $http.post(buildUrl(url), data).then(function (response) {
                var retVal = response.data;
                trackTime(startTime, url, data, 'post');
                //if (!disableClearCache) {
                //    cacheFactory.clearAll();
                //    //angular.forEach(ttls, function (ttl) {
                //    //    ttl.removeAll();
                //    //});
                //}
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

        //function getHtml(url) {
        //    var dfd = $q.defer(),
        //        startTime = new Date().getTime();
        //    if (cancelObjs[url]) {
        //        cancelObjs[url].resolve();
        //    }

        //    cancelObjs[url] = $q.defer();
        //    var getObj = {};
        //    getObj.timeout = cancelObjs[url].promise;
        //    url = buildUrl(url);
        //    url = routerHelper.buildUrl(url);
        //    $http.get(url, getObj).then(function (response) {
        //        trackTime(startTime, url, 'get html');
        //        var data = response.data;
        //        if (!data) {
        //            dfd.reject();
        //            return;
        //        }
        //        cancelObjs[url] = null;
        //        dfd.resolve(data);
        //        return;

        //    }, function (response) {
        //        dfd.reject(response);
        //        logError(url, response);
        //    });
        //    return dfd.promise;
        //}
        function get(url, data, ttl, disableCancel) {
            var dfd = $q.defer(),
                startTime = new Date().getTime();

            var getObj = {
                params: data
            };


            if (cancelObjs[url]) {
                cancelObjs[url].resolve();
            }

            if (!disableCancel) {
                cancelObjs[url] = $q.defer();
                getObj.timeout = cancelObjs[url].promise;
            }
            //if (ttl) {
            //    //ttl = ttl || 45000;
            //    getObj.cache = getCache(ttl);
            //}

            $http.get(buildUrl(url), getObj).then(function (response) {
                //var data = response.data;
                var retVal = response.data;
                trackTime(startTime, url, data, 'get');

                if (!retVal) {
                    dfd.reject();
                    return;
                }
                cancelObjs[url] = null;
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


        //function getCache(ttl) {
        //    var ttlString = ttl.toString();

        //    var dataCache = cacheFactory.get(ttlString);
        //    if (!dataCache) {
        //        dataCache = cacheFactory(ttlString, {
        //            maxAge: ttl
        //        });
        //    }
        //    return dataCache;
        //}
        
        return {
            get: get,
            post: post
     //       getHtml: getHtml
        };

    }
})();