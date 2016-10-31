﻿module app {
    "use strict";
    export interface IAppController {
        back(defaultUrl: string);
        logOut();
        toggleMenu();
        showToaster(text: string, parentId?: string);
        openMenu($mdOpenMenu: any, ev: Event);
        resetForm(myform: angular.IFormController);
    }

    class AppController implements IAppController {
        static $inject = ["$rootScope", "$location",
            "userDetailsFactory", "$mdToast", "$document", "$mdMenu", "resManager",
            "CacheFactory",
            "sbHistory", "$state", "dashboardService", "$urlRouter"];


        private showBoxAd: boolean;


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
            private dashboardService: IDashboardService,
            private $urlRouter: angular.ui.IUrlRouterService
        ) {

            // directive with menu
            // $rootScope.$on("$mdMenuClose", () => {
            //    this.menuOpened = false;
            // });

            // this.showMenu = true;
            this.showBoxAd = false;

            $rootScope.$on("$stateChangeSuccess", (event: angular.IAngularEvent, toState: angular.ui.IState
               /* toParams: ISpitballStateParamsService*/) => {
                this.showBoxAd = toState.parent === "box";
                var path = $location.path(),
                    absUrl = $location.absUrl(),
                    virtualUrl = absUrl.substring(absUrl.indexOf(path));
                window["dataLayer"].push({ event: "virtualPageView", virtualUrl: virtualUrl }); // google tag manger
                __insp.push(["virtualPage"]); // inspectlet
            });

            $rootScope.$on("$stateChangeError",
                (event: angular.IAngularEvent,
                    toState: angular.ui.IState, toParams: angular.ui.IStateParamsService,
                    fromState: angular.ui.IState, fromParams: angular.ui.IStateParamsService, error) => {
                    console.error(error);
                });

            $rootScope.$on("$stateChangeStart",
                (event: angular.IAngularEvent, toState: angular.ui.IState,
                    toParams: spitaball.ISpitballStateParamsService, fromState: angular.ui.IState,
                    fromParams: spitaball.ISpitballStateParamsService) => {
                    //if (!fromState.name) {
                    //    return;
                    //}

                    //$urlRouter.sync();
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
                    //if (toStateName !== "searchinfo") {
                    //    $rootScope.$broadcast("search-close");
                    //}
                    // TODO remove this to box controller
                    if (fromParams.boxId && toParams.boxId) {
                        if (fromParams.boxId === toParams.boxId && toStateName === "box"
                            && fromState.name.startsWith("box")) {
                            event.preventDefault();
                            $rootScope.$broadcast("state-change-start-prevent");
                        }
                    }

                    // TODO remove this to settings controller
                    if (toStateName === "settings" && fromState.name.startsWith("settings")) {
                        event.preventDefault();
                        $rootScope.$broadcast("state-change-start-prevent");
                    }
                    checkUniversityChoose();
                    //checkNumberOfBoxes();

                    function checkUniversityChoose() {
                        const details = userDetails.get();
                        if (details) {
                            if (!userDetails.isAuthenticated()) {
                                return;
                            }
                             // TODO remove that to university choose controller
                            if (!details.university.id) {
                                var userWithNoUniversityState = "universityChoose";
                                if (toStateName !== userWithNoUniversityState) {
                                    $rootScope.$broadcast("state-change-start-prevent");
                                    event.preventDefault();
                                }
                                return;
                            } else {
                                checkNumberOfBoxes(); 
                            }
                        } else {
                            event.preventDefault();
                            userDetails.init()
                                .then(() => {
                                    $urlRouter.sync();

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
                            if (dashboardService.boxes.length < 3 && toState.name !== "classChoose") {
                                event.preventDefault();
                                $rootScope.$broadcast("state-change-start-prevent");
                                $state.go("classChoose");
                            } else {
                                document.title = resManager.get("siteName");
                            }
                        } else {
                            event.preventDefault();
                            dashboardService.getBoxes()
                                .then(() => {
                                    $urlRouter.sync();

                                });
                        }
                    }
                    //event.preventDefault();

                });

        }
        
        back = (defaultUrl: string) => {
            var element = this.sbHistory.popElement();
            if (!element) {
                this.$location.url(defaultUrl);
                return;
            }
            this.$rootScope.$broadcast("from-back");
            this.$state.go(element.name, element.params);
        };

        logOut = () => {
            // we want to remove the user data and not the html
            sessionStorage.clear();
            //this.cacheFactory.clearAll();
            Intercom("shutdown");
        };

        toggleMenu = () => {
            this.$rootScope.$broadcast("open-menu");
        };

        showToaster = (text: string, parentId: string) => {
            let element: Element = this.$document.find("header")[0];
            if (parentId) {
                element = this.$document[0].querySelector(`#${parentId}`);
            }
            var toaster: any = this.$mdToast.simple()
                .textContent(text)
                .position("top right")
                .parent(element)

                //.theme(theme)
                .hideDelay(2000);
            // typedef doesnt have definition of toastClass
            toaster.toastClass("angular-animate");
            this.$mdToast.show(toaster);
        };


        openMenu = ($mdOpenMenu: any, ev: Event) => {
            // this.menuOpened = true;
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

