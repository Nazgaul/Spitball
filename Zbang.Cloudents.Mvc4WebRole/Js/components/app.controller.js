var app;
(function (app) {
    "use strict";
    var AppController = (function () {
        function AppController($rootScope, $location, userDetails, $mdToast, $document, $mdMenu, resManager, cacheFactory, $scope, realtimeFactotry, sbHistory, $state) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.$location = $location;
            this.userDetails = userDetails;
            this.$mdToast = $mdToast;
            this.$document = $document;
            this.$mdMenu = $mdMenu;
            this.resManager = resManager;
            this.cacheFactory = cacheFactory;
            this.$scope = $scope;
            this.realtimeFactotry = realtimeFactotry;
            this.sbHistory = sbHistory;
            this.$state = $state;
            this.chatDisplayState = 1;
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
                _this.cacheFactory.clearAll();
                Intercom("shutdown");
            };
            this.initChat = function () {
                var details = _this.userDetails.get();
                _this.loadChat = details.university.id > 0;
            };
            this.setTheme = function () {
                _this.theme = "theme-" + _this.userDetails.get().theme;
            };
            this.toggleMenu = function () {
                _this.$rootScope.$broadcast("open-menu");
            };
            this.showToaster = function (text, parentId, theme) {
                var element = _this.$document.find("header")[0];
                if (parentId) {
                    element = _this.$document[0].querySelector("#" + parentId);
                }
                _this.$mdToast.show(_this.$mdToast.simple()
                    .textContent(text)
                    .capsule(true)
                    .position("top right")
                    .parent(element)
                    .theme(theme)
                    .hideDelay(2000));
            };
            this.openMenu = function ($mdOpenMenu, ev) {
                _this.menuOpened = true;
                if (!_this.userDetails.isAuthenticated()) {
                    _this.$rootScope.$broadcast("show-unregisterd-box");
                    return;
                }
                $mdOpenMenu(ev);
            };
            $rootScope.$on("$viewContentLoaded", function () {
                var path = $location.path(), absUrl = $location.absUrl(), virtualUrl = absUrl.substring(absUrl.indexOf(path));
                dataLayer.push({ event: "virtualPageView", virtualUrl: virtualUrl });
                __insp.push(["virtualPage"]);
            });
            userDetails.init().then(function () {
                _this.setTheme();
                _this.initChat();
            });
            $rootScope.$on("$mdMenuClose", function () {
                _this.menuOpened = false;
            });
            this.showMenu = this.showSearch = this.showChat = true;
            this.showBoxAd = false;
            $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams) {
                _this.showBoxAd = toState.parent === "box";
                _this.showChat = _this.showSearch = !(toState.name === "universityChoose");
                _this.showMenu = !(toState.name === "item" || toState.name === "quiz" || toState.name === "universityChoose");
                if (toState.name.startsWith("box")) {
                    realtimeFactotry.assingBoxes(toParams.boxId);
                }
            });
            $rootScope.$on("$stateChangeStart", function (event, toState, toParams, fromState, fromParams) {
                if (!fromState.name) {
                    return;
                }
                if (toState.name === "user" && toParams.userId === "22886") {
                    event.preventDefault();
                    $rootScope.$broadcast("state-change-start-prevent");
                }
                $mdMenu.hide();
                $mdToast.hide();
                $rootScope.$broadcast("close-menu");
                $rootScope.$broadcast("close-collapse");
                var toStateName = toState.name;
                if (toStateName !== "searchinfo") {
                    $rootScope.$broadcast("search-close");
                }
                if (fromParams.boxId && toParams.boxId) {
                    if (fromParams.boxId === toParams.boxId && toStateName === "box" && fromState.name.startsWith("box")) {
                        event.preventDefault();
                        $rootScope.$broadcast("state-change-start-prevent");
                    }
                }
                if (toStateName === "settings" && fromState.name.startsWith("settings")) {
                    event.preventDefault();
                    $rootScope.$broadcast("state-change-start-prevent");
                }
                if (!userDetails.isAuthenticated()) {
                    return;
                }
                var details = userDetails.get();
                if (details.university.id) {
                    document.title = resManager.get("siteName");
                    return;
                }
                var userWithNoUniversityState = "universityChoose";
                if (toStateName !== userWithNoUniversityState) {
                    $rootScope.$broadcast("state-change-start-prevent");
                    event.preventDefault();
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
            "CacheFactory", "$scope", "realtimeFactotry", "sbHistory", "$state"];
        return AppController;
    }());
    angular.module("app").controller("AppController", AppController);
})(app || (app = {}));
