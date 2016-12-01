/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../shared/userDetails.ts" />
/// <reference path="../spitball.d.ts" />
module app {
    'use strict';

    var loaded = false;
    class SideMenu {
        static $inject = ["user", "dashboardService", "$location", "$scope", "$mdSidenav","$state"];

        courses = [];
        privateBoxes = [];

        constructor(private user: IUserData,
            private dashboardService: IDashboardService,
            private $location: angular.ILocationService,
            private $scope: angular.IScope,
            private $mdSidenav: angular.material.ISidenavService,
            private $state: angular.ui.IStateService
        ) {
            loaded = false; //loaded need to be initialize
            $scope.$on("close-menu", () => {
                $mdSidenav("left").close();
                //$scope.app.menuOpened = false;
                //self.menuOpened = !self.menuOpened;
            });
            $scope.$on("open-menu", () => {
                $mdSidenav("left").toggle();
            });
            $scope.$on("remove-box", (e, arg) => {
                arg = parseInt(arg, 10);
                this.removeElement(this.courses, arg);
                this.removeElement(this.privateBoxes, arg);
            });
            $scope.$on("$stateChangeSuccess", (event, toState, toParams, fromState, fromParams) => {
                if (fromState.parent === "box") {
                    if (fromParams.boxtype === "box") {
                        const box = this.privateBoxes.find(i => (i.id === fromParams.boxId)) || {};
                        box.updates = 0;
                    } else {
                        const abox = this.courses.find(i => (i.id === fromParams.boxId)) || {};
                        abox.updates = 0;
                    }

                }
            });
            $scope.$watch(() => {
                if (dashboardService.boxes) {
                    return dashboardService.boxes.length;
                }
                return dashboardService.boxes;
            }, val => {
                if (angular.isNumber(val)) {
                    if (val > 0) {
                        this.showBoxesNodes = true;
                    } else {
                        this.showBoxesNodes = false;
                    }
                    return;
                }
                if (!val) {
                    //we don't know what is the status
                    this.showBoxesNodes = true;
                }
            });
        }
        userUrl = this.$state.href("user", { userId: this.user.id, userName: this.user.name });
        showBoxesNodes = true;
        coursesOpen = false;
        boxesOpen = false;

        univeristyClick() {
            this.dashboardService.getUniversityMeta().then((response) => {
                this.$location.path(decodeURIComponent(response.url));
            });
        }
        initOpen() {
            if (!loaded) {
                this.getBoxes();
                loaded = true;
            }
            return true;
        }
        toggleCourses() {
            if (!this.initOpen()) {
                return;
            }
            this.coursesOpen = !this.coursesOpen;
            this.boxesOpen = false;
        }
        toggleBoxes() {
            if (!this.initOpen()) {
                return;
            }
            this.coursesOpen = false;
            this.boxesOpen = !this.boxesOpen;
        }


        isSectionSelected(section) {
            return decodeURI(this.$location.url()).startsWith(section);
        }

        private getBoxes() {
            this.dashboardService.getBoxes().then(response2 => {
                for (let i = 0; i < response2.length; i++) {
                    const b = response2[i];
                    if (b.boxType.startsWith('academic')) {
                        this.courses.push(b);
                    } else {
                        this.privateBoxes.push(b);
                    }
                }

            });
        }



        private removeElement(arr, arg) {
            const box = arr.find(e => (e.id === arg));
            if (box) {
                const index = arr.indexOf(box);
                arr.splice(index, 1);
            }
        }

       

    }

    angular.module("app").controller("SideMenu", SideMenu);
}
