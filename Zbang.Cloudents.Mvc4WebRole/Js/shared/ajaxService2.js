var app;
(function (app) {
    "use strict";
    var second = 1000, minute = 60 * second, hour = 60 * minute, day = 24 * hour;
    var cancelObjs = {};
    var AjaxService2 = (function () {
        function AjaxService2($http, $q, analytics, cacheFactory, routerHelper) {
            this.$http = $http;
            this.$q = $q;
            this.analytics = analytics;
            this.cacheFactory = cacheFactory;
            this.routerHelper = routerHelper;
            this.cacheCategories = {
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
            var dChat = window['dChat'];
            if (dChat.indexOf("develop") > -1) {
                this.cacheCategories.html = {
                    maxAge: 1 * second,
                    storageMode: "localStorage"
                };
            }
            for (var cacheKey in this.cacheCategories) {
                this.buildFactoryObject(cacheKey);
            }
        }
        AjaxService2.prototype.buildFactoryObject = function (cacheKey) {
            var dst = {};
            angular.extend(dst, {
                deleteOnExpire: "aggressive",
                maxAge: minute,
                recycleFreq: 15000,
                storageMode: "sessionStorage",
                storagePrefix: version
            }, this.cacheCategories[cacheKey]);
            this.cacheFactory(cacheKey, dst);
        };
        AjaxService2.prototype.deleteCacheCategory = function (category) {
            var dataCache = this.cacheFactory.get(category);
            dataCache.removeAll();
        };
        AjaxService2.prototype.post = function (url, data, category) {
            var _this = this;
            var dfd = this.$q.defer(), startTime = new Date().getTime();
            this.$http.post(this.buildUrl(url), data).then(function (response) {
                var retVal = response.data;
                _this.trackTime(startTime, url, data, "post");
                if (angular.isArray(category)) {
                    category.forEach(function (e) {
                        _this.deleteCacheCategory(e);
                    });
                }
                if (angular.isString(category)) {
                    _this.deleteCacheCategory(category);
                }
                if (!retVal) {
                    _this.logError(url, data, retVal);
                    dfd.reject();
                    return;
                }
                if (retVal.success) {
                    dfd.resolve(retVal.payload);
                    return;
                }
                dfd.reject(retVal.payload);
            }).catch(function (response) {
                dfd.reject(response);
                _this.logError(url, data, response);
            });
            return dfd.promise;
        };
        AjaxService2.prototype.getHtml = function (url) {
            var _this = this;
            var dfd = this.$q.defer();
            url = this.buildUrl(url);
            url = this.routerHelper.buildUrl(url);
            var dataCache = this.cacheFactory.get("html");
            if (dataCache.get(url)) {
                dfd.resolve(dataCache.get(url));
            }
            else {
                var startTime = new Date().getTime();
                this.$http.get(url).then(function (response) {
                    _this.trackTime(startTime, url, "get html", "html");
                    var data = response.data;
                    if (!data) {
                        dfd.reject();
                        return;
                    }
                    dataCache.put(url, data);
                    dfd.resolve(data);
                    return;
                }).catch(function (response) {
                    dfd.reject(response);
                    _this.logError(url, null, response);
                });
            }
            return dfd.promise;
        };
        AjaxService2.prototype.get = function (url, data, category, cancelCategory) {
            var self = this;
            var deferred = this.$q.defer();
            var cacheKey = url + JSON.stringify(data);
            if (category) {
                var dataCache = this.cacheFactory.get(category);
                if (dataCache && dataCache.get(cacheKey)) {
                    deferred.resolve(dataCache.get(cacheKey));
                    return deferred.promise;
                }
            }
            getFromServer(category);
            return deferred.promise;
            function getFromServer(cacheCategory) {
                var startTime = new Date().getTime();
                var getObj = {
                    params: data
                };
                if (cancelCategory) {
                    if (cancelObjs[cancelCategory]) {
                        cancelObjs[cancelCategory].resolve();
                    }
                    cancelObjs[cancelCategory] = self.$q.defer();
                    getObj.timeout = cancelObjs[cancelCategory].promise;
                }
                self.$http.get(self.buildUrl(url), getObj).then(function (response) {
                    var retVal = response.data;
                    self.trackTime(startTime, url, data, "get");
                    if (!retVal) {
                        deferred.reject();
                        return;
                    }
                    if (retVal.success) {
                        if (cacheCategory) {
                            var categoryFactory = self.cacheFactory.get(cacheCategory);
                            categoryFactory.put(cacheKey, retVal.payload);
                        }
                        deferred.resolve(retVal.payload);
                        return;
                    }
                    deferred.reject(retVal.payload);
                }).catch(function (response) {
                    deferred.reject(response);
                    self.logError(url, data, response);
                });
            }
        };
        AjaxService2.prototype.buildUrl = function (url) {
            url = url.toLowerCase();
            if (!url.startsWith("/")) {
                url = "/" + url;
            }
            if (!url.endsWith("/")) {
                url = url + "/";
            }
            ;
            return url;
        };
        AjaxService2.prototype.logError = function (url, data, payload) {
            var log = {
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
        };
        AjaxService2.prototype.trackTime = function (startTime, url, data, type) {
            var timeSpent = new Date().getTime() - startTime;
            this.analytics.trackTimings(url.toLowerCase() !== "/item/preview/" ? "ajax " + type
                : "ajaxPreview", url, timeSpent, JSON.stringify(data));
        };
        AjaxService2.$inject = ["$http", "$q", "Analytics", "CacheFactory", "routerHelper"];
        return AjaxService2;
    }());
    angular.module("app").service("ajaxService2", AjaxService2);
})(app || (app = {}));
