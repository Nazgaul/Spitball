/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />


declare var version: any;

module app {
    "use strict";
    import Factory = CacheFactory.ICacheFactory;

    export interface IAjaxService2 {
        get(url: string, data?: Object, category?: cacheKeys, cancelCategory?: string): angular.IPromise<Object | Array<any> | number>;
        post(url: string, data: Object, category?: cacheKeys | Array<cacheKeys>): angular.IPromise<Object | Array<any>>;
        put(url: string, data: Object, category: cacheKeys | Array<cacheKeys>): angular.IPromise<any>;
        getHtml(url: string): angular.IPromise<any>;
        deleteCacheCategory(category: cacheKeys): void;
        logError(url: string, data?: Object, payload?: Object): void;
    }

    export type cacheKeys = "university" |
        "accountDetail" |
        "html" |
        "department" |
        "itemComment" |
        "boxItems" |
        "boxData" |
        "searchBox" | "searchItem" | "searchQuiz" | "searchFirstBox";

    const
        second = 1000,
        minute = 60 * second,
        hour = 60 * minute,
        day = 24 * hour;
    var cancelObjs = {};
    class AjaxService2 implements IAjaxService2 {
        static $inject = ["$http", "$q", "Analytics", "CacheFactory", "routerHelper"];

        constructor(private $http: angular.IHttpService,
            private $q: angular.IQService,
            private analytics: IAnalytics,
            private cacheFactory: Factory,
            private routerHelper: IRouterHelper) {


            const dChat: string = window['dChat'];
            if (dChat.indexOf("develop") > -1) {
                this.cacheCategories.html = {
                    maxAge: 1 * second,
                    storageMode: "localStorage"
                };
            }
            for (let cacheKey in this.cacheCategories) {
                this.buildFactoryObject(cacheKey);
            }
        }
        private cacheCategories = {
            university: {
                maxAge: 6 * hour
            },
            accountDetail: {
                maxAge: day
            },
            searchItem: {
                maxAge: hour
            },
            searchBox: {
                maxAge: hour
            },
            searchFirstBox: {
                maxAge: hour
            },
            searchQuiz: {
                maxAge: hour
            },
            html: {
                maxAge: 30 * day,
                storageMode: "localStorage"
            },
            department: {
                maxAge: 15 * minute
            },
            boxItems: {
                maxAge: 15 * minute
            },
            boxData: {
                maxAge: 15 * minute
            },
            itemComment: {
                maxAge: 15 * minute
            }
        };
        buildFactoryObject(cacheKey: string) {
            const dst = {};
            angular.extend(dst,
                {
                    deleteOnExpire: "aggressive",
                    maxAge: minute,
                    recycleFreq: 15000, // 15 seconds
                    storageMode: "sessionStorage",
                    storagePrefix: version
                }, this.cacheCategories[cacheKey]);
            this.cacheFactory(cacheKey, dst);
        }

        deleteCacheCategory(category: cacheKeys) {
            const dataCache = this.cacheFactory.get(category);
            dataCache.removeAll();
        }
        private insertUpdate(func, url: string, data: Object, category: cacheKeys | Array<cacheKeys>): angular.IPromise<any> {
            var dfd = this.$q.defer(),
                startTime = new Date().getTime();

            func(this.buildUrl(url), data).then((response: angular.IHttpPromiseCallbackArg<{}>) => {
                var retVal: any = response.data;
                this.trackTime(startTime, url, data, "post");

                if (angular.isArray(category)) {
                    (category as Array<cacheKeys>).forEach((e: cacheKeys) => {
                        this.deleteCacheCategory(e);
                    });
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

        put(url: string, data: Object, category: cacheKeys | Array<cacheKeys>): angular.IPromise<any> {
            return this.insertUpdate(this.$http.put, url, data, category);
        }

        post(url: string, data: Object, category: cacheKeys | Array<cacheKeys>): angular.IPromise<any> {
            return this.insertUpdate(this.$http.post, url, data, category);
            //var dfd = this.$q.defer(),
            //    startTime = new Date().getTime();

            //this.$http.post(this.buildUrl(url), data).then((response: angular.IHttpPromiseCallbackArg<{}>) => {
            //    var retVal: any = response.data;
            //    this.trackTime(startTime, url, data, "post");

            //    if (angular.isArray(category)) {
            //        (category as Array<cacheKeys>).forEach((e: cacheKeys) => {
            //            this.deleteCacheCategory(e);
            //        });
            //    }
            //    if (angular.isString(category)) {
            //        this.deleteCacheCategory(category as cacheKeys);
            //    }

            //    if (!retVal) {
            //        this.logError(url, data, retVal);
            //        dfd.reject();
            //        return;
            //    }
            //    if (retVal.success) {
            //        dfd.resolve(retVal.payload);
            //        return;
            //    }

            //    dfd.reject(retVal.payload);

            //}).catch((response: any) => {
            //    dfd.reject(response);
            //    this.logError(url, data, response);
            //});
            //return dfd.promise;
        }

        getHtml(url: string): angular.IPromise<any> {
            var dfd = this.$q.defer();
            url = this.buildUrl(url);
            url = this.routerHelper.buildUrl(url);
            var dataCache = this.cacheFactory.get("html");
            if (dataCache.get(url)) {
                dfd.resolve(dataCache.get(url));
            } else {
                var startTime = new Date().getTime();
                this.$http.get(url).then((response: angular.IHttpPromiseCallbackArg<{}>) => {
                    this.trackTime(startTime, url, "get html", "html");
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

            }
            getFromServer(category);
            return deferred.promise;


            function getFromServer(cacheCategory) {
                var startTime = new Date().getTime();

                const getObj: any = {
                    params: data
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
                    self.trackTime(startTime, url, data, "get");
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

            if (!url.startsWith("/")) {
                url = `/${url}`;
            }
            if (!url.endsWith("/")) {
                url = `${url}/`;
            };
            return url;
        }


        logError(url: string, data: Object, payload: Object) {
            const log = {
                data: data,
                payload: payload
            };
            console.error("eror ajax", url, data, payload);
            $.ajax({
                type: "POST",
                url: "/error/jslog/",
                contentType: "application/json",
                data: angular.toJson({
                    errorUrl: url,
                    errorMessage: JSON.stringify(log),
                    cause: "ajaxRequest"
                })
            });
        }

        trackTime(startTime: number, url: string, data: Object, type: string) {
            const timeSpent = new Date().getTime() - startTime;

            this.analytics.trackTimings(url.toLowerCase() !== "/item/preview/" ? `ajax ${type}`
                : "ajaxPreview", url, timeSpent, JSON.stringify(data));
        }
    }
    angular.module("app").service("ajaxService2", AjaxService2);
}

