//declare var version: any;
//module app {
//    "use strict";
//    export interface IAjaxService {
//        get(url: string, data?: Object): angular.IPromise<Object | Array<any> | number>;
//        post(url: string, data: Object): angular.IPromise<Object | Array<any>>;
//        put(url: string, data: Object): angular.IPromise<any>;
//        delete(url: string, data: Object): angular.IPromise<any>;
//        getHtml(url: string): angular.IPromise<any>;
//        logError(url: string, data?: Object, payload?: Object): void;
//    }
//    const
//        second = 1000,
//        minute = 60 * second,
//        hour = 60 * minute,
//        day = 24 * hour;
//    class AjaxService implements IAjaxService {
//        static $inject = ["$http", "$q"];
//        constructor(private $http: angular.IHttpService,
//            private $q: angular.IQService) { }
//        private insertUpdate(func, url: string, data: Object): angular.IPromise<any> {
//            var dfd = this.$q.defer(),
//                startTime = new Date().getTime();
//            func(this.buildUrl(url), data)
//                .then((response: angular.IHttpPromiseCallbackArg<{}>) => {
//                    var retVal: any = response.data;
//                    this.trackTime(startTime, url, data, "post");
//                    if (!retVal) {
//                        this.logError(url, data, retVal);
//                        dfd.reject();
//                        return;
//                    }
//                    if (retVal.success) {
//                        dfd.resolve(retVal.payload);
//                        return;
//                    }
//                    if (response.status === 200) {
//                        dfd.reject(retVal.payload);
//                        return;
//                    }
//                    dfd.reject(response)
//                        ;
//                }).catch((response: any) => {
//                    dfd.reject(response);
//                    this.logError(url, data, response);
//                });
//            return dfd.promise;
//        }
//        put(url: string, data: Object): angular.IPromise<any> {
//            return this.insertUpdate(this.$http.put, url, data);
//        }
//        delete(url: string, data: Object): angular.IPromise<any> {
//            return this.insertUpdate(this.$http.delete, url, { params: data });
//        }
//        post(url: string, data: Object): angular.IPromise<any> {
//            return this.insertUpdate(this.$http.post, url, data);
//        }
//        getHtml(url: string): angular.IPromise<any> {
//            var dfd = this.$q.defer();
//            url = this.buildUrl(url);
//                var startTime = new Date().getTime();
//                this.$http.get(url).then((response: angular.IHttpPromiseCallbackArg<{}>) => {
//                    var data = response.data;
//                    if (!data) {
//                        dfd.reject();
//                        return;
//                    }
//                    dfd.resolve(data);
//                    return;
//                }).catch(response => {
//                    dfd.reject(response);
//                    this.logError(url, null, response);
//                });
//            return dfd.promise;
//        }
//        get(url: string, data: Object): angular.IPromise<any> {
//            var self = this;
//            var deferred = this.$q.defer();
//            var cacheKey = url + JSON.stringify(data);
//            getFromServer();
//            return deferred.promise;
//            function getFromServer() {
//                var startTime = new Date().getTime();
//                const getObj: any = {
//                    params: data
//                };
//                self.$http.get(self.buildUrl(url), getObj).then((response: angular.IHttpPromiseCallbackArg<{}>) => {
//                    var retVal: any = response.data;
//                    self.trackTime(startTime, url, data, "get");
//                    if (!retVal) {
//                        deferred.reject();
//                        return;
//                    }
//                    if (retVal.success) {
//                        deferred.resolve(retVal.payload);
//                        return;
//                    }
//                    deferred.reject(retVal.payload);
//                }).catch(response => {
//                    deferred.reject(response);
//                    self.logError(url, data, response);
//                });
//            }
//        }
//        buildUrl(url: string) {
//            url = url.toLowerCase();
//            if (!url.startsWith("/")) {
//                url = `/${url}`;
//            }
//            if (!url.endsWith("/")) {
//                url = `${url}/`;
//            };
//            return url;
//        }
//        logError(url: string, data: Object, payload: Object) {
//            const log = {
//                data: data,
//                payload: payload
//            };
//            console.error("eror ajax", url, data, payload);
//            $.ajax({
//                type: "POST",
//                url: "/error/jslog/",
//                contentType: "application/json",
//                data: angular.toJson({
//                    errorUrl: url,
//                    errorMessage: JSON.stringify(log),
//                    cause: "ajaxRequest"
//                })
//            });
//        }
//        trackTime(startTime: number, url: string, data: Object, type: string) {
//            const timeSpent = new Date().getTime() - startTime;
//        }
//    }
//    angular.module("app").service("ajaxService", AjaxService);
//}
//# sourceMappingURL=ajaxService.js.map