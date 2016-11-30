module app {
    "use strict";
    export interface IAppController {
        back(defaultUrl: string);
        //logOut();
        toggleMenu();
        //showToaster(text: string, parentId?: string);
        openMenu($mdOpenMenu: any, ev: Event);
        //resetForm(myform: angular.IFormController);
    }

    class AppController implements IAppController {
        static $inject = ["$rootScope", "$location",
            "userDetailsFactory", "$mdToast", "$document", "$mdMenu", "resManager",
            "CacheFactory",
            "sbHistory", "$state", "dashboardService"];

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
            private dashboardService: IDashboardService
        ) {
            $rootScope.$on("$stateChangeSuccess", () => {
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
                    toParams: spitaball.ISpitballStateParamsService) => {
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
                        const details = userDetails.get();
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
                            } else {
                                checkNumberOfBoxes();
                            }
                        } else {
                            event.preventDefault();
                            userDetails.init()
                                .then(() => {
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
                            } else {
                                document.title = resManager.get("siteName");
                            }
                        } else {
                            event.preventDefault();
                            $rootScope.$broadcast("state-change-start-prevent");
                            dashboardService.getBoxes()
                                .then(() => {
                                    $state.go(toState, toParams);
                                });
                        }
                    }
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

       

        toggleMenu = () => {
            this.$rootScope.$broadcast("open-menu");
        };

        openMenu = ($mdOpenMenu: any, ev: Event) => {
            if (!this.userDetails.isAuthenticated()) {
                this.$rootScope.$broadcast("show-unregisterd-box");
                return;
            }
            $mdOpenMenu(ev);
        };

        //resetForm(myform: angular.IFormController) {
        //    myform.$setPristine();
        //    myform.$setUntouched();
        //};
    }
    angular.module("app").controller("AppController", AppController);

}

