/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
'use strict';

interface String {
    startsWith(str: string): boolean;
    endsWith(str: string): boolean;
}
interface IAnalytics extends angular.google.analytics.AnalyticsService
{
    trackTimings(timingCategory: string, timingVar: string, timingValue: number, timingLabel:string)
}


interface IAjaxService2 {
    get(url: string, data?: Object, category?: cacheKeys, cancelCategory?: string): angular.IPromise<any>;
    post(url: string, data: Object, category?: cacheKeys | Array<cacheKeys>): angular.IPromise<any>;
    getHtml(url: string): angular.IPromise<any>;
    deleteCacheCategory(category: cacheKeys): void;
}
//declare module 'cacheKeys' {
type cacheKeys = 'university' | 'accountDetail' | 'html' | 'department' | 'itemComment' | 'chat'
//}
module app {
    'use strict';
    import Factory = CacheFactory.ICacheFactory;

    export interface IAjaxService2 {
        get(url: string, data?: Object, category?: cacheKeys, cancelCategory?: string): angular.IPromise<any>;
        post(url: string, data: Object, category?: cacheKeys | Array<cacheKeys>): angular.IPromise<any>;
        getHtml(url: string): angular.IPromise<any>;
        deleteCacheCategory(category: cacheKeys): void;
    }
    //declare module 'cacheKeys' {
    export type cacheKeys = 'university' | 'accountDetail' | 'html' | 'department' | 'itemComment' | 'chat'

    const minute = 60 * 1000,
        hour = 60 * minute,
        day = 24 * hour;
    var cancelObjs = {}
    class AjaxService2 implements IAjaxService2 {
        static $inject = ['$http', '$q', 'Analytics', 'CacheFactory', 'routerHelper'];

        constructor(private $http: angular.IHttpService,
            private $q: angular.IQService,
            private analytics: IAnalytics,
            private cacheFactory: Factory,
            private routerHelper: IRouterHelper) {

            for (let cacheKey in this.cacheCategories) {
                this.buildFactoryObject(cacheKey);
            }
        }
        //private const minute = 60 * 1000;
        //hour = 60 * minute,
        //day = 24 * hour,
        //cancelObjs = {},
        private cacheCategories = {
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



        buildFactoryObject(cacheKey: string) {
            var dst = {};
            angular.extend(dst,
                {
                    deleteOnExpire: 'aggressive',
                    maxAge: minute,
                    recycleFreq: 15000, // 15 seconds
                    storageMode: 'sessionStorage',
                    storagePrefix: 'sb.c.'
                }, this.cacheCategories[cacheKey]);
            this.cacheFactory(cacheKey, dst);
        }

        deleteCacheCategory(category: cacheKeys) {
            const dataCache = this.cacheFactory.get(category);
            //if (dataCache) {
            dataCache.removeAll();
            //}
        }

        post(url: string, data: Object, category: cacheKeys | Array<cacheKeys>): angular.IPromise<any> {
            var dfd = this.$q.defer(),
                startTime = new Date().getTime();

            this.$http.post(this.buildUrl(url), data).then((response: angular.IHttpPromiseCallbackArg<{}>) => {
                var retVal: any = response.data;
                this.trackTime(startTime, url, data, 'post');

                if (angular.isArray(category)) {
                    (category as Array<cacheKeys>).forEach(e => {
                        this.deleteCacheCategory(e);
                    });
                    //for (let cat in category) {
                    //    if (category.hasOwnProperty(cat)) {
                    //        deleteCategory(category[cat]);
                    //    }
                    //}
                }
                if (angular.isString(category)) {
                    this.deleteCacheCategory(category as cacheKeys);
                }

                if (!retVal) {
                    this.logError(url, data, retVal);
                    dfd.reject();
                    return;
                }
                if (retVal.success) {
                    dfd.resolve(retVal.payload);
                    return;
                }

                dfd.reject(retVal.payload);

            }).catch((response: any) => {
                dfd.reject(response);
                this.logError(url, data, response);
            });
            return dfd.promise;
        }

        getHtml(url: string): angular.IPromise<any> {
            var dfd = this.$q.defer();
            url = this.buildUrl(url);
            url = this.routerHelper.buildUrl(url);
            var dataCache = this.cacheFactory.get('html');
            if (dataCache.get(url)) {
                dfd.resolve(dataCache.get(url));
            } else {
                var startTime = new Date().getTime();
                this.$http.get(url).then((response: angular.IHttpPromiseCallbackArg<{}>) => {
                    this.trackTime(startTime, url, 'get html', 'html');
                    var data = response.data;
                    if (!data) {
                        dfd.reject();
                        return;
                    }
                    dataCache.put(url, data);
                    dfd.resolve(data);
                    return;
                }).catch(response => {
                    dfd.reject(response);
                    this.logError(url, null, response);
                });

            }
            return dfd.promise;
        }
        get(url: string, data: Object, category: cacheKeys, cancelCategory: string): angular.IPromise<any> {
            var self = this;
            var deferred = this.$q.defer();
            var cacheKey = url + JSON.stringify(data);

            if (category) {
                const dataCache = this.cacheFactory.get(category);
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

                const getObj: any = {
                    params: data
                    //timeout: cancelObjs[cancelCategory]
                };
                if (cancelCategory) {
                    if (cancelObjs[cancelCategory]) {
                        cancelObjs[cancelCategory].resolve();
                    }
                    cancelObjs[cancelCategory] = self.$q.defer();
                    getObj.timeout = cancelObjs[cancelCategory].promise;
                }

                self.$http.get(self.buildUrl(url), getObj).then((response: angular.IHttpPromiseCallbackArg<{}>) => {
                    var retVal: any = response.data;
                    self.trackTime(startTime, url, data, 'get');
                    //delete cancelObjs[cancelCategory];
                    if (!retVal) {
                        deferred.reject();
                        return;
                    }
                    if (retVal.success) {
                        if (cacheCategory) {
                            const categoryFactory = self.cacheFactory.get(cacheCategory);
                            categoryFactory.put(cacheKey, retVal.payload);
                        }
                        deferred.resolve(retVal.payload);
                        return;
                    }
                    deferred.reject(retVal.payload);
                }).catch(response => {
                    deferred.reject(response);
                    self.logError(url, data, response);
                });
            }

        }
        buildUrl(url: string) {
            url = url.toLowerCase();

            if (!url.startsWith('/')) {
                url = `/${url}`;
            }
            if (!url.endsWith('/')) {
                url = url + '/';
            };
            return url;
        }


        private logError(url: string, data, payload) {
            const log = {
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

        trackTime(startTime, url, data, type) {
            var timeSpent = new Date().getTime() - startTime;

            this.analytics.trackTimings(url.toLowerCase() !== '/item/preview/' ? 'ajax ' + type
                : 'ajaxPreview', url, timeSpent, JSON.stringify(data));
        }
    }
    angular.module('app').service('ajaxService2', AjaxService2);
}

//(() => {
//    angular.module('app.ajaxservice').factory('ajaxService2', ajaxService);
//    ajaxService.$inject = ['$http', '$q', 'Analytics', 'CacheFactory', 'routerHelper'];


//    function ajaxService($http: angular.IHttpService, $q: angular.IQService, analytics: any, cacheFactory: any, routerHelper: IRouterHelper): IAjaxService2 {
//        const minute = 60 * 1000,
//            hour = 60 * minute,
//            day = 24 * hour,
//            cancelObjs = {},
//            cacheCategories = {
//                university: {
//                    maxAge: 6 * hour,
//                    storageMode: 'localStorage'
//                },
//                accountDetail: {
//                    maxAge: day

//                },
//                html: {
//                    maxAge: 30 * day,
//                    storageMode: 'localStorage'
//                },
//                department: {
//                    maxAge: 15 * minute
//                },
//                itemComment: {
//                    maxAge: 15 * minute
//                },
//                chat: {
//                    maxAge: 30 * minute
//                }
//            };
//        for (let cacheKey in cacheCategories) {
//            if (cacheCategories.hasOwnProperty(cacheKey)) {
//                buildFactoryObject(cacheKey);
//            }
//        }

//        function buildFactoryObject(cacheKey) {
//            angular.extend(cacheCategories[cacheKey],
//                {
//                    deleteOnExpire: 'aggressive',
//                    maxAge: minute,
//                    recycleFreq: 15000, // 15 seconds
//                    storageMode: 'sessionStorage',
//                    storagePrefix: 'sb.c.'
//                });

//            cacheFactory(cacheKey, cacheCategories[cacheKey]);
//        }

//        function deleteCategory(category: cacheKeys) {
//            const dataCache = cacheFactory.get(category);
//            //if (dataCache) {
//            dataCache.removeAll();
//            //}
//        }

//        function post(url: string, data: Object, category: cacheKeys | Array<cacheKeys>) {
//            var dfd = $q.defer(),
//                startTime = new Date().getTime();

//            $http.post(buildUrl(url), data).then(response => {
//                var retVal: any = response.data;
//                trackTime(startTime, url, data, 'post');

//                if (angular.isArray(category)) {
//                    (category as Array<cacheKeys>).forEach(e => {
//                        deleteCategory(e);
//                    });
//                    //for (let cat in category) {
//                    //    if (category.hasOwnProperty(cat)) {
//                    //        deleteCategory(category[cat]);
//                    //    }
//                    //}
//                }
//                if (angular.isString(category)) {
//                    deleteCategory(category as cacheKeys);
//                }

//                if (!retVal) {
//                    logError(url, data, retVal);
//                    dfd.reject();
//                    return;
//                }
//                if (retVal.success) {
//                    dfd.resolve(retVal.payload);
//                    return;
//                }

//                dfd.reject(retVal.payload);

//            }, response => {
//                dfd.reject(response);
//                logError(url, data, response);
//            });
//            return dfd.promise;
//        }

//        function getHtml(url: string) {
//            var dfd = $q.defer();
//            url = buildUrl(url);
//            url = routerHelper.buildUrl(url);
//            var dataCache = cacheFactory.get('html');
//            if (dataCache.get(url)) {
//                dfd.resolve(dataCache.get(url));
//            } else {
//                var startTime = new Date().getTime();
//                $http.get(url).then(response => {
//                    trackTime(startTime, url, 'get html', 'html');
//                    var data = response.data;
//                    if (!data) {
//                        dfd.reject();
//                        return;
//                    }
//                    dataCache.put(url, data);
//                    dfd.resolve(data);
//                    return;
//                }, response => {
//                    dfd.reject(response);
//                    logError(url, null, response);
//                });

//            }
//            return dfd.promise;
//        }
//        function get(url: string, data: Object, category: cacheKeys, cancelCategory: string) {
//            var deferred = $q.defer();
//            var cacheKey = url + JSON.stringify(data);

//            if (category) {
//                const dataCache = cacheFactory.get(category);
//                if (dataCache && dataCache.get(cacheKey)) {    // if there is factory fot the current cateegory - bring the cache
//                    deferred.resolve(dataCache.get(cacheKey));
//                    return deferred.promise;
//                }
//                //return deferred.promise;

//            }
//            getFromServer(category);
//            return deferred.promise;


//            function getFromServer(cacheCategory) {
//                var startTime = new Date().getTime();

//                var getObj: any = {
//                    params: data
//                    //timeout: cancelObjs[cancelCategory]
//                };
//                if (cancelCategory) {
//                    if (cancelObjs[cancelCategory]) {
//                        cancelObjs[cancelCategory].resolve();
//                    }
//                    cancelObjs[cancelCategory] = $q.defer();
//                    getObj.timeout = cancelObjs[cancelCategory].promise;
//                }

//                $http.get(buildUrl(url), getObj).then(response => {
//                    var retVal: any = response.data;
//                    trackTime(startTime, url, data, 'get');
//                    //delete cancelObjs[cancelCategory];
//                    if (!retVal) {
//                        deferred.reject();
//                        return;
//                    }
//                    if (retVal.success) {
//                        if (cacheCategory) {
//                            var categoryFactory = cacheFactory.get(cacheCategory);
//                            categoryFactory.put(cacheKey, retVal.payload);
//                        }
//                        deferred.resolve(retVal.payload);
//                        return;
//                    }
//                    deferred.reject(retVal.payload);
//                }, response => {
//                    deferred.reject(response);
//                    logError(url, data, response);
//                });
//            }

//        }
//        function buildUrl(url) {
//            url = url.toLowerCase();
//            if (!url.startsWith('/')) {
//                url = '/' + url;
//            }
//            if (!url.endsWith('/')) {
//                url = url + '/';
//            };
//            return url;
//        }


//        function logError(url: string, data, payload) {
//            var log = {
//                data: data,
//                payload: payload
//            };

//            $.ajax({
//                type: 'POST',
//                url: '/error/jslog/',
//                contentType: 'application/json',
//                data: angular.toJson({
//                    errorUrl: url,
//                    errorMessage: JSON.stringify(log),
//                    cause: 'ajaxRequest',
//                    stackTrace: ''
//                })
//            });
//        }

//        function trackTime(startTime, url, data, type) {
//            var timeSpent = new Date().getTime() - startTime;

//            analytics.trackTimings(url.toLowerCase() !== '/item/preview/' ? 'ajax ' + type
//                : 'ajaxPreview', url, timeSpent, JSON.stringify(data));
//        }

//        return {
//            get: get,
//            post: post,
//            getHtml: getHtml,
//            deleteCacheCategory: deleteCategory
//        };

//    }
//})();