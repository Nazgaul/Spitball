var app;
(function (app) {
    "use strict";
    var UserUpdatesService = (function () {
        function UserUpdatesService(ajaxService, $q, userDetailsFactory, $rootScope, $window, $stateParams) {
            var _this = this;
            this.ajaxService = ajaxService;
            this.$q = $q;
            this.userDetailsFactory = userDetailsFactory;
            this.$rootScope = $rootScope;
            this.$window = $window;
            this.$stateParams = $stateParams;
            this.allUpdates = {};
            this.deferred = $q.defer();
            userDetailsFactory.init()
                .then(function (userData) {
                if (userData.id)
                    _this.getUpdates();
            });
            $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
                if (fromState.parent === "box") {
                    _this.deleteUpdates(fromParams.boxId);
                }
            });
        }
        UserUpdatesService.prototype.getUpdates = function () {
            var _this = this;
            this.ajaxService.get("/account/updates/").then(function (response2) {
                _this.data = response2;
                for (var i = 0; i < response2.length; i++) {
                    var currBox = _this.allUpdates[response2[i].boxId] || {};
                    if (response2[i].answerId) {
                        currBox[response2[i].answerId] = true;
                    }
                    else if (response2[i].questionId) {
                        currBox[response2[i].questionId] = true;
                    }
                    _this.allUpdates[response2[i].boxId] = currBox;
                }
                _this.deferred.resolve();
            }).catch(function () {
                _this.deferred.reject();
            });
        };
        UserUpdatesService.prototype.deleteUpdates = function (boxId) {
            var _this = this;
            this.updatesNum(boxId).then(function (length) {
                if (!length) {
                    return;
                }
                _this.deleteFromServer(boxId);
                delete _this.allUpdates[boxId];
                _this.$rootScope.$broadcast("delete-updates", boxId);
            });
        };
        UserUpdatesService.prototype.deleteFromServer = function (boxId) {
            if (!this.userDetailsFactory.isAuthenticated()) {
                return;
            }
            this.ajaxService.post("/box/deleteupdates/", {
                boxId: boxId
            });
        };
        UserUpdatesService.prototype.updatesNum = function (boxid) {
            var _this = this;
            var q = this.$q.defer();
            this.boxUpdates(boxid).then(function () {
                q.resolve(_this.allUpdates[boxid] ? Object.keys(_this.allUpdates[boxid]).length : 0);
            });
            return q.promise;
        };
        UserUpdatesService.prototype.boxUpdates = function (boxid) {
            var _this = this;
            var promise = this.deferred.promise;
            var q = this.$q.defer();
            promise.then(function () {
                q.resolve(_this.allUpdates[boxid]);
            });
            return q.promise;
        };
        return UserUpdatesService;
    }());
    UserUpdatesService.$inject = ["ajaxService2", "$q", "userDetailsFactory", "$rootScope", "$window", "$stateParams"];
    angular.module('app.user').service('userUpdatesService', UserUpdatesService);
})(app || (app = {}));
//# sourceMappingURL=updates.service.js.map