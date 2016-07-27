'use strict';
(function () {
    angular.module('app.ajaxservice').factory('ajaxService2', ajaxService);
    ajaxService.$inject = ['$http', '$q', 'Analytics', 'CacheFactory', 'routerHelper'];


    function ajaxService($http, $q, analytics, cacheFactory, routerHelper) {
        var minute = 60 * 1000;
        var hour = 60 * minute;
        var day = 24 * hour;
        var cancelObjs = {};
        var cacheCategories = {
            university: {
                maxAge: 6 * 60 * minute
            },
            html: {
                maxAge: 30 * day
            },
            department : {
                maxAge: 15 * minute,
                storageMode: 'sessionStorage'
            },
            itemComment: {
                maxAge: 15 * minute,
                storageMode: 'sessionStorage'
            }
        };

        for (var cacheKey in cacheCategories) {
            buildFactoryObject(cacheKey);
        }

        function buildFactoryObject(cacheKey) {
            //if (!cacheCategories[cacheKey]) {
            //    return;
            //}
            angular.extend(cacheCategories[cacheKey],
            {
                deleteOnExpire: 'aggressive',
                maxAge: minute,
                recycleFreq: 15000, // 15 seconds
                storageMode: 'localStorage',
                storagePrefix: 'sb.c.'
            });

            cacheFactory(cacheKey, cacheCategories[cacheKey]);
        }

        function deleteCategory(category) {
            var dataCache = cacheFactory.get(category);
            dataCache.removeAll();
        }

        function post(url, data, category) {
            var dfd = $q.defer(),
                startTime = new Date().getTime();

            $http.post(buildUrl(url), data).then(function (response) {
                var retVal = response.data;
                trackTime(startTime, url, data, 'post');
                if (angular.isArray(category)) {
                    for (var cat in category) {
                        deleteCategory(cat);
                    }
                }
                else if (category) {
                    deleteCategory(category);
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
            var dfd = $q.defer();
            url = buildUrl(url);
            url = routerHelper.buildUrl(url);
            var dataCache = cacheFactory.get('html');
            if (dataCache.get(url)) {
                dfd.resolve(dataCache.get(url));
            } else {
                var startTime = new Date().getTime();
                $http.get(url).then(function (response) {
                    trackTime(startTime, url, 'get html');
                    var data = response.data;
                    if (!data) {
                        dfd.reject();
                        return;
                    }
                    dataCache.put(url, data);
                    dfd.resolve(data);
                    return;
                }, function (response) {
                    dfd.reject(response);
                    logError(url, response);
                });

            }
            return dfd.promise;
        }
        function get(url, data, category, cancelCategory) {
            var deferred = $q.defer();
            var cacheKey = url + JSON.stringify(data);

            if (category) {
                var dataCache = cacheFactory.get(category);
                if (dataCache && dataCache.get(cacheKey)) {    // if there is factory fot the current cateegory - bring the cache
                    deferred.resolve(dataCache.get(cacheKey));
                    return deferred.promise;
                }
                //return deferred.promise;

            }
            getFromServer(category);
            return deferred.promise;


            function getFromServer(cacheCategory) {
                var startTime = new Date().getTime();

                var getObj = {
                    params: data
                    //timeout: cancelObjs[cancelCategory]
                };
                if (cancelCategory) {
                    if (cancelObjs[cancelCategory]) {
                        cancelObjs[cancelCategory].resolve();
                    }
                    cancelObjs[cancelCategory] = $q.defer();
                    getObj.timeout = cancelObjs[cancelCategory].promise;
                }

                $http.get(buildUrl(url), getObj).then(function (response) {
                    var retVal = response.data;
                    trackTime(startTime, url, data, 'get');
                    //delete cancelObjs[cancelCategory];
                    if (!retVal) {
                        deferred.reject();
                        return;
                    }
                    if (retVal.success) {
                        if (cacheCategory) {
                            var categoryFactory = cacheFactory.get(cacheCategory);
                            categoryFactory.put(cacheKey, retVal.payload);
                        }
                        deferred.resolve(retVal.payload);
                        return;
                    }
                    deferred.reject(retVal.payload);
                }, function (response) {
                    deferred.reject(response);
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
        }

        return {
            get: get,
            post: post,
            getHtml: getHtml
        };

    }
})();