var app;
(function (app) {
    "use strict";
    var AppController = (function () {
        function AppController($rootScope, $location, userDetails, $mdToast, $document, $mdMenu, resManager, cacheFactory, sbHistory, $state, $window, $timeout) {
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
            this.$window = $window;
            this.$timeout = $timeout;
            this.back = function (defaultUrl) {
                var element = _this.sbHistory.popElement();
                console.log(element);
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
            //initChat = () => {
            //    var details = this.userDetails.get();
            //    this.loadChat = details.university.id > 0;
            //};
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
            //directive with menu
            userDetails.init().then(function () {
                _this.setTheme();
            });
            //directive with menu
            $rootScope.$on("$mdMenuClose", function () {
                _this.menuOpened = false;
            });
            //this.showMenu = true;
            this.showBoxAd = false;
            $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams) {
                _this.showBoxAd = toState.parent === "box";
                // hub
                //if (toState.name.startsWith("box")) {
                //    realtimeFactotry.assingBoxes(toParams.boxId);
                //}
                if (toParams["pageYOffset"]) {
                    $timeout(function () {
                        $window.scrollTo(0, toParams["pageYOffset"]);
                    });
                }
                var path = $location.path(), absUrl = $location.absUrl(), virtualUrl = absUrl.substring(absUrl.indexOf(path));
                window["dataLayer"].push({ event: "virtualPageView", virtualUrl: virtualUrl }); // google tag manger
                __insp.push(["virtualPage"]); // inspectlet
            });
            $rootScope.$on('$stateChangeError', function (event, toState, toParams, fromState, fromParams, error) {
                console.error(error);
            });
            $rootScope.$on("$stateChangeStart", function (event, toState, toParams, fromState, fromParams) {
                if (!fromState.name) {
                    return;
                }
                // can't access anonymous user
                if (toState.name === "user" && toParams.userId === 22886) {
                    event.preventDefault();
                    $rootScope.$broadcast("state-change-start-prevent");
                }
                $mdMenu.hide(); // closes menu
                $mdToast.hide(); // hide toasters
                $rootScope.$broadcast("close-menu");
                $rootScope.$broadcast("close-collapse");
                var toStateName = toState.name;
                if (toStateName !== "searchinfo") {
                    $rootScope.$broadcast("search-close");
                }
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
            "CacheFactory",
            "sbHistory", "$state", "$window", "$timeout"];
        return AppController;
    }());
    angular.module("app").controller("AppController", AppController);
})(app || (app = {}));
//# sourceMappingURL=app.controller.js.map