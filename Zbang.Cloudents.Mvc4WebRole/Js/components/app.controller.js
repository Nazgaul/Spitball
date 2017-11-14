"use strict";
var app;
(function (app) {
    "use strict";
    var AppController = (function () {
        function AppController($rootScope, $location, userDetails, $mdToast, $document, $mdMenu, resManager, cacheFactory, sbHistory, $state, dashboardService, $log) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.$location = $location;
            this.userDetails = userDetails;
            this.$mdToast = $mdToast;
            this.$document = $document;
            this.$mdMenu = $mdMenu;
            this.resManager = resManager;
            this.cacheFactory = cacheFactory;
            this.sbHistory = sbHistory;
            this.$state = $state;
            this.dashboardService = dashboardService;
            this.$log = $log;
            this.back = function (defaultUrl) {
                var element = _this.sbHistory.popElement();
                if (!element) {
                    _this.$location.url(defaultUrl);
                    return;
                }
                _this.$rootScope.$broadcast("from-back");
                _this.$state.go(element.name, element.params);
            };
            this.toggleMenu = function () {
                _this.$rootScope.$broadcast("open-menu");
            };
            this.openMenu = function ($mdOpenMenu, ev) {
                if (!_this.userDetails.isAuthenticated()) {
                    _this.$rootScope.$broadcast("show-unregisterd-box");
                    return;
                }
                $mdOpenMenu(ev);
            };
            $rootScope.$on("$stateChangeSuccess", function () {
                var path = $location.path(), absUrl = $location.absUrl(), virtualUrl = absUrl.substring(absUrl.indexOf(path));
                window["dataLayer"].push({ event: "virtualPageView", virtualUrl: virtualUrl }); // google tag manger
                __insp.push(["virtualPage"]); // inspectlet
            });
            $rootScope.$on("$stateChangeError", function (event, toState, toParams, fromState, fromParams, error) {
                $log.error(error);
            });
            $rootScope.$on("$stateChangeStart", function (event, toState, toParams) {
                // can't access anonymous user
                if (toState.name === "user" && toParams.userId === 22886) {
                    event.preventDefault();
                    $rootScope.$broadcast("state-change-start-prevent");
                }
                $mdMenu.hide(); // closes menu
                $mdToast.hide(); // hide toasters
                $rootScope.$broadcast("close-menu");
                $rootScope.$broadcast("close-collapse");
                checkUniversityChoose();
                function checkUniversityChoose() {
                    var details = userDetails.get();
                    if (details) {
                        if (!userDetails.isAuthenticated()) {
                            return;
                        }
                        // TODO remove that to university choose controller
                        if (!details.university.id) {
                            var userWithNoUniversityState = "universityChoose";
                            if (toState.name !== userWithNoUniversityState) {
                                $rootScope.$broadcast("state-change-start-prevent");
                                event.preventDefault();
                            }
                            return;
                        }
                        else {
                            checkNumberOfBoxes();
                        }
                    }
                    else {
                        event.preventDefault();
                        userDetails.init()
                            .then(function () {
                            $state.go(toState, toParams);
                        });
                    }
                }
                function checkNumberOfBoxes() {
                    if (!userDetails.isAuthenticated()) {
                        return;
                    }
                    if (toState.name === "classChoose") {
                        return;
                    }
                    if (dashboardService.boxes) {
                        if (dashboardService.boxes.length < 2 && toState.name !== "classChoose") {
                            event.preventDefault();
                            $rootScope.$broadcast("state-change-start-prevent");
                            $state.go("classChoose");
                        }
                        else {
                            document.title = resManager.get("siteName");
                        }
                    }
                    else {
                        event.preventDefault();
                        $rootScope.$broadcast("state-change-start-prevent");
                        dashboardService.getBoxes()
                            .then(function () {
                            $state.go(toState, toParams);
                        });
                    }
                }
            });
        }
        return AppController;
    }());
    AppController.$inject = ["$rootScope", "$location",
        "userDetailsFactory", "$mdToast", "$document", "$mdMenu", "resManager",
        "CacheFactory",
        "sbHistory", "$state", "dashboardService", "$log"];
    angular.module("app").controller("AppController", AppController);
})(app || (app = {}));
//# sourceMappingURL=app.controller.js.map