﻿
interface ISpitballStateParamsService extends angular.ui.IStateParamsService {
    boxId: number;
    itemId: number;
    userId: number;
}
module app {
    "use strict";
    class AppController {
        static $inject = ["$rootScope", "$location",
            "userDetailsFactory", "$mdToast", "$document", "$mdMenu", "resManager",
            "CacheFactory", 
            "sbHistory", "$state", "$window", "$timeout"];

        private menuOpened: boolean;
        private showMenu: boolean;
        private showChat: boolean;
        private showBoxAd: boolean;
        private theme: string;


        constructor(private $rootScope: angular.IRootScopeService,
            private $location: angular.ILocationService,
            private userDetails: IUserDetailsFactory,
            private $mdToast: angular.material.IToastService,
            private $document: angular.IDocumentService,
            private $mdMenu: angular.material.IMenuService,
            private resManager: IResManager,
            private cacheFactory: CacheFactory.ICacheFactory,
            private sbHistory: ISbHistory,
            private $state: angular.ui.IStateService,
            private $window: angular.IWindowService,
            private $timeout: angular.ITimeoutService
        ) {



            //directive with menu
            userDetails.init().then(() => {
                this.setTheme();
            });

            //directive with menu
            $rootScope.$on("$mdMenuClose", () => {
                this.menuOpened = false;
            });

            //this.showMenu = true;
            this.showBoxAd = false;

            $rootScope.$on("$stateChangeSuccess", (event: angular.IAngularEvent, toState: angular.ui.IState,
                toParams: ISpitballStateParamsService) => {
                this.showBoxAd = toState.parent === "box";

                // hub
                //if (toState.name.startsWith("box")) {
                //    realtimeFactotry.assingBoxes(toParams.boxId);
                //}
                if (toParams["pageYOffset"]) {
                    $timeout(() => {
                        $window.scrollTo(0, toParams["pageYOffset"]);
                    });
                }
                var path = $location.path(),
                    absUrl = $location.absUrl(),
                    virtualUrl = absUrl.substring(absUrl.indexOf(path));
                window["dataLayer"].push({ event: "virtualPageView", virtualUrl: virtualUrl }); // google tag manger
                __insp.push(["virtualPage"]); // inspectlet
            });

            $rootScope.$on('$stateChangeError',
                (event, toState, toParams, fromState, fromParams, error) => {
                    console.error(error);
                });

            $rootScope.$on("$stateChangeStart",
                (event: angular.IAngularEvent, toState: angular.ui.IState,
                    toParams: ISpitballStateParamsService, fromState: angular.ui.IState, fromParams: ISpitballStateParamsService) => {
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
        back = (defaultUrl: string) => {
            var element = this.sbHistory.popElement();
            console.log(element);
            if (!element) {
                this.$location.url(defaultUrl);
                return;
            }
            this.$rootScope.$broadcast("from-back");
            this.$state.go(element.name, element.params);
        };

        logOut = () => {
            this.cacheFactory.clearAll();
            Intercom("shutdown");
        };
        //initChat = () => {
        //    var details = this.userDetails.get();
        //    this.loadChat = details.university.id > 0;
        //};

        setTheme = () => {
            this.theme = `theme-${this.userDetails.get().theme}`;
        };

        toggleMenu = () => {
            this.$rootScope.$broadcast("open-menu");
        };

        showToaster = (text: string, parentId: string, theme: string) => {
            let element: Element = this.$document.find("header")[0];
            if (parentId) {
                element = this.$document[0].querySelector(`#${parentId}`);
            }

            this.$mdToast.show(
                this.$mdToast.simple()
                    .textContent(text)
                    .capsule(true)
                    .position("top right")
                    .parent(element)
                    .theme(theme)
                    .hideDelay(2000));
        };


        openMenu = ($mdOpenMenu: any, ev: Event) => {
            this.menuOpened = true;
            if (!this.userDetails.isAuthenticated()) {
                this.$rootScope.$broadcast("show-unregisterd-box");
                return;
            }
            $mdOpenMenu(ev);
        };

        resetForm(myform: angular.IFormController) {
            myform.$setPristine();
            myform.$setUntouched();
        };
    }
    angular.module("app").controller("AppController", AppController);

}

