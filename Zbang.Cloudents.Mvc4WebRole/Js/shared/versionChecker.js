var app;
(function (app) {
    "use strict";
    var timeInterval = 900000;
    var VerionChecker = (function () {
        function VerionChecker($http, cacheFactory, $mdToast, resManager, $interval) {
            var _this = this;
            this.$http = $http;
            this.cacheFactory = cacheFactory;
            this.$mdToast = $mdToast;
            this.resManager = resManager;
            this.$interval = $interval;
            if (document.readyState === "complete") {
                $interval(this.checkVersion, timeInterval);
            }
            else {
                window.addEventListener("load", function () {
                    _this.checkVersion();
                    $interval(function () {
                        _this.checkVersion();
                    }, timeInterval);
                }, false);
            }
        }
        VerionChecker.prototype.checkVersion = function () {
            var _this = this;
            this.$http.get("/home/version/").then(function (response) {
                var retVal = response.data;
                if (retVal.success) {
                    if (window["version"] === retVal.payload) {
                        return;
                    }
                    _this.cacheFactory.clearAll();
                    var toast = _this.$mdToast.simple().textContent(_this.resManager.get("spitballUpdate")).action(_this.resManager.get('dialogOk'))
                        .highlightAction(false)
                        .position("top");
                    _this.$mdToast.show(toast).then(function () {
                        window.location.reload(true);
                    });
                }
            });
        };
        VerionChecker.factory = function () {
            var factory = function ($http, cacheFactory, $mdToast, resManager, $interval) {
                return new VerionChecker($http, cacheFactory, $mdToast, resManager, $interval);
            };
            factory["$inject"] = ["$http", "CacheFactory", "$mdToast", "resManager", "$interval"];
            return factory;
        };
        return VerionChecker;
    }());
    angular.module('app').run(VerionChecker.factory());
})(app || (app = {}));
