/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../scripts/typings/angular-google-analytics/angular-google-analytics.d.ts" />
/// <reference path="ajaxservice2.ts" />
var app;
(function (app) {
    "use strict";
    var UserDetails = (function () {
        function UserDetails($rootScope, $q, ajaxService, analytics, $timeout, $interval) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.$q = $q;
            this.ajaxService = ajaxService;
            this.analytics = analytics;
            this.$timeout = $timeout;
            this.$interval = $interval;
            this.isLogedIn = false;
            this.serverCall = false;
            this.deferDetails = this.$q.defer();
            this.get = function () {
                return _this.userData;
            };
            this.isAuthenticated = function () {
                return _this.isLogedIn;
            };
            this.setName = function (first, last) {
                _this.userData.name = first + " " + last;
                _this.$rootScope.$broadcast("userDetailsChange");
            };
            this.setImage = function (image) {
                if (!image) {
                    return;
                }
                _this.userData.image = image;
                _this.$rootScope.$broadcast("userDetailsChange");
            };
            this.getUniversity = function () {
                return _this.userData ? _this.userData.university.id : null;
            };
            this.setUniversity = function () {
                _this.ajaxService.deleteCacheCategory("accountDetail");
                _this.userData = null;
                _this.deferDetails = _this.$q.defer();
                return _this.init();
            };
            this.setTheme = function (theme) {
                _this.userData.theme = theme;
            };
        }
        UserDetails.prototype.setDetails = function (data) {
            var _this = this;
            if (data.id) {
                this.isLogedIn = true;
                // ReSharper disable UseOfImplicitGlobalInFunctionScope
                __insp.push(["identify", data.id]);
            }
            this.$timeout(function () {
                //analytics doesnt work with timeout
                _this.analytics.set("dimension1", data.universityName || null);
                _this.analytics.set("dimension2", data.universityCountry || null);
                _this.analytics.set("dimension3", data.id || null);
            });
            //this.analytics.set("dimension4", data.theme || "dark");
            var interval = this.$interval(function () {
                if (googletag.pubads !== undefined && googletag.pubads) {
                    googletag.pubads().setTargeting("gender", data.sex);
                    googletag.pubads().setTargeting("university", data.universityId);
                    _this.$interval.cancel(interval);
                }
            }, 20);
            this.userData = {
                id: data.id,
                name: data.name,
                image: data.image,
                sex: data.sex,
                score: data.score,
                url: data.url,
                createTime: new Date(data.dateTime),
                isAdmin: data.isAdmin,
                theme: data.theme,
                culture: data.culture,
                email: data.email,
                unread: data.unread,
                university: {
                    country: data.universityCountry,
                    name: data.universityName,
                    id: data.universityId
                }
            };
        };
        UserDetails.prototype.init = function () {
            var _this = this;
            if (this.userData) {
                this.deferDetails.resolve(this.userData);
                return this.deferDetails.promise;
            }
            if (!this.serverCall) {
                this.serverCall = true;
                this.ajaxService.get("/account/details/", null, "accountDetail").then(function (response) {
                    _this.setDetails(response);
                    _this.deferDetails.resolve(_this.userData);
                    _this.serverCall = false;
                });
            }
            return this.deferDetails.promise;
        };
        UserDetails.$inject = ["$rootScope", "$q", "ajaxService2", "Analytics", "$timeout", "$interval"];
        return UserDetails;
    }());
    angular.module("app").service("userDetailsFactory", UserDetails);
})(app || (app = {}));
//# sourceMappingURL=userDetails.js.map