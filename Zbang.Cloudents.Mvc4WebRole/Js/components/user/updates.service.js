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
                    //if (userData.university.id) {
                    _this.getUpdates();
                //} else {
                //need to resolve this
                // this.deferred.resolve();
                //}
            });
            $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
                if (fromState.parent === "box") {
                    _this.deleteUpdates(fromParams.boxId);
                }
            });
            $window.onbeforeunload = function () {
                var boxId = $stateParams.boxId;
                if (boxId) {
                    _this.deleteFromServer(boxId);
                }
            };
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
                _this.$rootScope.$broadcast("refresh-boxes");
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
        UserUpdatesService.$inject = ["ajaxService2", "$q", "userDetailsFactory", "$rootScope", "$window", "$stateParams"];
        return UserUpdatesService;
    }());
    angular.module('app.user').service('userUpdatesService', UserUpdatesService);
})(app || (app = {}));
// (function () {
//    angular.module('app.user').service('userUpdatesService', userUpdates);
//    userUpdates.$inject = ['ajaxService', '$q', 'userDetailsFactory', '$rootScope', '$window', '$stateParams'];
//    function userUpdates(ajaxservice, $q, userDetails, $rootScope, $window, $stateParams) {
//        var self = this;
//        var data = [];
//        var deferred = $q.defer();
//        userDetails.init().then(function () {
//            if (userDetails.get().university.id) {
//                getUpdates();
//            }
//        });
//        self.updatesNum = updatesNum;
//        self.boxUpdates = boxUpdates;
//        self.deleteUpdates = deleteUpdates;
//        var allUpdates = {};
//        $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
//            if (fromState.parent === 'box') {
//                deleteUpdates(fromParams.boxId);
//            }
//        });
//        function getUpdates() {
//            ajaxservice.get('/user/updates/').then(function (response2) {
//                data = response2;
//                for (var i = 0; i < response2.length; i++) {
//                    //var currBox = typeof (allUpdates[response2[i].boxId]) == "undefined" ? {} : allUpdates[response2[i].boxId];
//                    var currBox = allUpdates[response2[i].boxId] || {};
//                    if (response2[i].answerId) {
//                        currBox[response2[i].answerId] = true;
//                    }
//                    else if (response2[i].questionId) {
//                        currBox[response2[i].questionId] = true;
//                    }
//                    allUpdates[response2[i].boxId] = currBox;
//                }
//                deferred.resolve();//(self.data);
//            });
//        }
//        $window.onbeforeunload = function () {
//            var boxId = $stateParams.boxId;
//            if (boxId) {
//                deleteFromServer(boxId);
//            }
//        };
//        function deleteFromServer(boxId) {
//            if (!userDetails.isAuthenticated()) {
//                return;
//            }
//            ajaxservice.post("/box/deleteupdates/", {
//                boxId: boxId
//            });
//        }
//        function deleteUpdates(boxId) {
//            boxId = parseInt(boxId, 10);
//            updatesNum(boxId).then(function (length) {
//                if (!length) {
//                    return;
//                }
//                deleteFromServer(boxId);
//                delete allUpdates[boxId];
//                $rootScope.$broadcast('refresh-boxes');
//            });
//        }
//        function updatesNum(boxid) {
//            var q = $q.defer();
//            boxUpdates(boxid).then(function () {
//                q.resolve(allUpdates[boxid] ? Object.keys(allUpdates[boxid]).length : 0);
//            });
//            return q.promise;
//        }
//        function boxUpdates(boxid) {
//            var promise = deferred.promise;
//            var q = $q.defer();
//            promise.then(function () {
//                q.resolve(allUpdates[boxid]);
//            });
//            return q.promise;
//        }
//    }
// })(); 
