/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
'use strict';


interface IAjaxService2 {
    get(url: string, data?: Object, category?: cacheKeys, cancelCategory?: string): angular.IPromise<any>;
    post(url: string, data: Object, category?: cacheKeys | Array<cacheKeys>): angular.IPromise<any>;
    getHtml(url: string): angular.IPromise<any>;
    deleteCacheCategory(category: cacheKeys): void;
}
//declare module 'cacheKeys' {
type cacheKeys = 'university' | 'accountDetail' | 'html' | 'department' | 'itemComment' | 'chat'
//}

(() => {
    angular.module('app.ajaxservice').factory('ajaxService2', ajaxService);
    ajaxService.$inject = ['$http', '$q', 'Analytics', 'CacheFactory', 'routerHelper'];


    function ajaxService($http: angular.IHttpService, $q: angular.IQService, analytics: any, cacheFactory: any, routerHelper: any): IAjaxService2 {
        const minute = 60 * 1000,
              hour = 60 * minute,
              day = 24 * hour,
             cancelObjs = {},
            cacheCategories = {
                university: {
                    maxAge: 6 * hour,
                    storageMode: 'localStorage'
                },
                accountDetail: {
                    maxAge: day

                },
                html: {
                    maxAge: 30 * day,
                    storageMode: 'localStorage'
                },
                department: {
                    maxAge: 15 * minute
                },
                itemComment: {
                    maxAge: 15 * minute
                },
                chat: {
                    maxAge: 30 * minute
                }
            };
        for (let cacheKey in cacheCategories) {
            if (cacheCategories.hasOwnProperty(cacheKey)) {
                buildFactoryObject(cacheKey);
            }
        }

        function buildFactoryObject(cacheKey) {
            angular.extend(cacheCategories[cacheKey],
            {
                deleteOnExpire: 'aggressive',
                maxAge: minute,
                recycleFreq: 15000, // 15 seconds
                storageMode: 'sessionStorage',
                storagePrefix: 'sb.c.'
            });

            cacheFactory(cacheKey, cacheCategories[cacheKey]);
        }

        function deleteCategory(category: cacheKeys) {
            const dataCache = cacheFactory.get(category);
            //if (dataCache) {
            dataCache.removeAll();
            //}
        }

        function post(url: string, data: Object, category: cacheKeys | Array<cacheKeys>) {
            var dfd = $q.defer(),
                startTime = new Date().getTime();

            $http.post(buildUrl(url), data).then(response => {
                var retVal: any = response.data;
                trackTime(startTime, url, data, 'post');
                
                if (angular.isArray(category)) {
                    (category as Array<cacheKeys>).forEach(e => {
                        deleteCategory(e);
                    });
                    //for (let cat in category) {
                    //    if (category.hasOwnProperty(cat)) {
                    //        deleteCategory(category[cat]);
                    //    }
                    //}
                }
                if (angular.isString(category)) {
                    deleteCategory(category as cacheKeys);
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

            }, response => {
                dfd.reject(response);
                logError(url, data, response);
            });
            return dfd.promise;
        }

        function getHtml(url: string) {
            var dfd = $q.defer();
            url = buildUrl(url);
            url = routerHelper.buildUrl(url);
            var dataCache = cacheFactory.get('html');
            if (dataCache.get(url)) {
                dfd.resolve(dataCache.get(url));
            } else {
                var startTime = new Date().getTime();
                $http.get(url).then(response => {
                    trackTime(startTime, url, 'get html', 'html');
                    var data = response.data;
                    if (!data) {
                        dfd.reject();
                        return;
                    }
                    dataCache.put(url, data);
                    dfd.resolve(data);
                    return;
                }, response => {
                    dfd.reject(response);
                    logError(url, null, response);
                });

            }
            return dfd.promise;
        }
        function get(url: string, data: Object, category: cacheKeys, cancelCategory: string) {
            var deferred = $q.defer();
            var cacheKey = url + JSON.stringify(data);

            if (category) {
                const dataCache = cacheFactory.get(category);
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

                var getObj:any = {
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

                $http.get(buildUrl(url), getObj).then(response => {
                    var retVal:any = response.data;
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
                }, response => {
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
            getHtml: getHtml,
            deleteCacheCategory: deleteCategory
        };

    }
})();