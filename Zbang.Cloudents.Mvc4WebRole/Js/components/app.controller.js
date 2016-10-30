var app;
(function (app) {
    "use strict";
    var AppController = (function () {
        function AppController($rootScope, $location, userDetails, $mdToast, $document, $mdMenu, resManager, cacheFactory, sbHistory, $state, dashboardService, $urlRouter) {
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
            this.$urlRouter = $urlRouter;
            this.back = function (defaultUrl) {
                var element = _this.sbHistory.popElement();
                if (!element) {
                    _this.$location.url(defaultUrl);
                    return;
                }
                _this.$rootScope.$broadcast("from-back");
                _this.$state.go(element.name, element.params);
            };
            this.logOut = function () {
                sessionStorage.clear();
                Intercom("shutdown");
            };
            this.toggleMenu = function () {
                _this.$rootScope.$broadcast("open-menu");
            };
            this.showToaster = function (text, parentId) {
                var element = _this.$document.find("header")[0];
                if (parentId) {
                    element = _this.$document[0].querySelector("#" + parentId);
                }
                var toaster = _this.$mdToast.simple()
                    .textContent(text)
                    .position("top right")
                    .parent(element)
                    .hideDelay(2000);
                toaster.toastClass("angular-animate");
                _this.$mdToast.show(toaster);
            };
            this.openMenu = function ($mdOpenMenu, ev) {
                if (!_this.userDetails.isAuthenticated()) {
                    _this.$rootScope.$broadcast("show-unregisterd-box");
                    return;
                }
                $mdOpenMenu(ev);
            };
            this.showBoxAd = false;
            $rootScope.$on("$stateChangeSuccess", function (event, toState) {
                _this.showBoxAd = toState.parent === "box";
                var path = $location.path(), absUrl = $location.absUrl(), virtualUrl = absUrl.substring(absUrl.indexOf(path));
                window["dataLayer"].push({ event: "virtualPageView", virtualUrl: virtualUrl });
                __insp.push(["virtualPage"]);
            });
            $rootScope.$on("$stateChangeError", function (event, toState, toParams, fromState, fromParams, error) {
                console.error(error);
            });
            $rootScope.$on("$stateChangeStart", function (event, toState, toParams, fromState, fromParams) {
                if (toState.name === "user" && toParams.userId === 22886) {
                    event.preventDefault();
                    $rootScope.$broadcast("state-change-start-prevent");
                }
                $mdMenu.hide();
                $mdToast.hide();
                $rootScope.$broadcast("close-menu");
                $rootScope.$broadcast("close-collapse");
                var toStateName = toState.name;
                if (fromParams.boxId && toParams.boxId) {
                    if (fromParams.boxId === toParams.boxId && toStateName === "box"
                        && fromState.name.startsWith("box")) {
                        event.preventDefault();
                        $rootScope.$broadcast("state-change-start-prevent");
                    }
                }
                if (toStateName === "settings" && fromState.name.startsWith("settings")) {
                    event.preventDefault();
                    $rootScope.$broadcast("state-change-start-prevent");
                }
                checkUniversityChoose();
                checkNumberOfBoxes();
                function checkUniversityChoose() {
                    var details = userDetails.get();
                    if (details) {
                        if (!userDetails.isAuthenticated()) {
                            return;
                        }
                        if (!details.university.id) {
                            var userWithNoUniversityState = "universityChoose";
                            if (toStateName !== userWithNoUniversityState) {
                                $rootScope.$broadcast("state-change-start-prevent");
                                event.preventDefault();
                            }
                            return;
                        }
                    }
                    else {
                        event.preventDefault();
                        userDetails.init()
                            .then(function () {
                            $urlRouter.sync();
                        });
                    }
                }
                function checkNumberOfBoxes() {
                    if (!userDetails.isAuthenticated()) {
                        return;
                    }
                    if (dashboardService.boxes) {
                        if (dashboardService.boxes.length < 3 && toState.name !== "classChoose") {
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
                        dashboardService.getBoxes()
                            .then(function () {
                            $urlRouter.sync();
                        });
                    }
                }
            });
        }
        AppController.prototype.resetForm = function (myform) {
            myform.$setPristine();
            myform.$setUntouched();
        };
        ;
        AppController.$inject = ["$rootScope", "$location",
            "userDetailsFactory", "$mdToast", "$document", "$mdMenu", "resManager",
            "CacheFactory",
            "sbHistory", "$state", "dashboardService", "$urlRouter"];
        return AppController;
    }());
    angular.module("app").controller("AppController", AppController);
})(app || (app = {}));
