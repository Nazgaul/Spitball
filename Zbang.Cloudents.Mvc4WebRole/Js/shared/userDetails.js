"use strict";
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
                _this.$rootScope.$broadcast("refresh-university");
                _this.userData = null;
                _this.deferDetails = _this.$q.defer();
                return _this.init();
            };
            this.updateScore = function (score) {
                _this.userData.score = score;
                _this.ajaxService.deleteCacheCategory("accountDetail");
                _this.$rootScope.$broadcast("userDetailsChange");
            };
            this.updateBadge = function () {
                _this.userData.badges++;
                _this.ajaxService.deleteCacheCategory("accountDetail");
                _this.$rootScope.$broadcast("userDetailsChange");
            };
        }
        UserDetails.prototype.setDetails = function (data) {
            var _this = this;
            if (data.id) {
                this.isLogedIn = true;
                // ReSharper disable UseOfImplicitGlobalInFunctionScope
                __insp.push(["identify", data.id]);
                // ReSharper restore UseOfImplicitGlobalInFunctionScope
            }
            this.$timeout(function () {
                //analytics doesnt work without timeout
                _this.analytics.set("dimension1", data.universityName || "null"); // analytics doesnt support null
                _this.analytics.set("dimension2", data.universityCountry || "null"); // analytics doesnt support null
                _this.analytics.set("dimension3", data.id || "null"); // analytics doesnt support null
            });
            this.userData = {
                id: data.id,
                name: data.name,
                image: data.image,
                score: data.score,
                createTime: new Date(data.dateTime),
                isAdmin: data.isAdmin,
                culture: data.culture,
                badges: data.badges,
                email: data.email,
                levelName: data.levelName,
                nextLevel: data.nextLevel,
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
        return UserDetails;
    }());
    UserDetails.$inject = ["$rootScope", "$q", "ajaxService2", "Analytics", "$timeout", "$interval"];
    angular.module("app").service("userDetailsFactory", UserDetails);
})(app || (app = {}));
//# sourceMappingURL=userDetails.js.map