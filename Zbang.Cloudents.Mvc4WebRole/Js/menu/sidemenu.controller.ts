/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../shared/userDetails.ts" />
/// <reference path="../spitball.d.ts" />
module app {
    'use strict';

    var loaded = false;
    class SideMenu {
        static $inject = ["user", "dashboardService", "$location", "$scope", "$mdSidenav"];

        courses = [];
        privateBoxes = [];

        constructor(private user: IUserData,
            private dashboardService: IDashboardService,
            private $location: angular.ILocationService,
            private $scope: angular.IScope,
            private $mdSidenav: angular.material.ISidenavService
           ) {
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
        userUrl = this.user.url;
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
                for (var i = 0; i < response2.length; i++) {
                    var b = response2[i];
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

//(function () {
//    angular.module('app.dashboard').controller('SideMenu', sideMenu);
//    sideMenu.$inject = ['dashboardService', 'userDetailsFactory',
//        '$mdSidenav', '$location', '$scope'];

//    function sideMenu(dashboardService, userDetailsFactory,
//        $mdSidenav, $location, $scope) {
//        var d = this, loaded = false;
//        d.courses = [];
//        d.privateBoxes = [];

//        var userDetails = userDetailsFactory.get();
//        d.userUrl = userDetails.url;
//        if (userDetails.theme === 'dark') {
//            d.updateScrollbar = {
//                theme: 'light-thin'
//                //scrollbarPosition: 'outside'
//            };
//        }

//        d.showBoxesNodes = true;
//        d.coursesOpen = false;
//        d.boxesOpen = false;

//        d.isSectionSelected = isSectionSelected;

//        d.toggleCourses = toggleCourses;
//        d.toggleBoxes = toggleBoxes;
//        d.univeristyClick = univeristyClick;

//        function univeristyClick() {
//            dashboardService.getUniversityMeta()
//                .then(function (response) {
//                    $location.path(decodeURIComponent(response.url));
//                });
//        }

//        function toggleCourses() {
//            if (!initOpen()) {
//                return;
//            }
//            d.coursesOpen = !d.coursesOpen;
//            d.boxesOpen = false;

//        }
//        function toggleBoxes() {
//            if (!initOpen()) {
//                return;
//            }
//            d.coursesOpen = false;
//            d.boxesOpen = !d.boxesOpen;
//        }
//        function initOpen() {
//            if (!loaded) {
//                getBoxes();
//                loaded = true;
//            }
//            return true;
//        }

//        function isSectionSelected(section) {
//            return $location.url().startsWith(section);
//        }

//        function getBoxes() {
//            dashboardService.getBoxes().then(function (response2) {
//                for (var i = 0; i < response2.length; i++) {
//                    var b = response2[i];
//                    if (b.boxType.startsWith('academic')) {
//                        d.courses.push(b);
//                    } else {
//                        d.privateBoxes.push(b);
//                    }
//                }

//            });
//        }

//        $scope.$on('close-menu', function () {
//            $mdSidenav('left').close();
//            //$scope.app.menuOpened = false;
//            self.menuOpened = !self.menuOpened;
//        });
//        $scope.$on('open-menu', function () {
//            $mdSidenav('left').toggle();
//            //$scope.app.menuOpened = !$scope.app.menuOpened;
//        });
//        $scope.$on('remove-box', function (e, arg) {
//            arg = parseInt(arg, 10);
//            removeElement(d.courses, arg);
//            removeElement(d.privateBoxes, arg);
//        });

//        function removeElement(arr, arg) {
//            var box = arr.find(function (e) {
//                return e.id === arg;
//            });
//            if (box) {
//                var index = arr.indexOf(box);
//                arr.splice(index, 1);
//            }
//        }

//        $scope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
//            if (fromState.parent === 'box') {
//                if (fromParams.boxtype === 'box') {
//                    var box = d.privateBoxes.find(function (i) {
//                        // ReSharper disable once CoercedEqualsUsing
//                        return i.id == fromParams.boxId;
//                    }) || {};
//                    box.updates = 0;
//                } else {
//                    var abox = d.courses.find(function (i) {
//                        // ReSharper disable once CoercedEqualsUsing
//                        return i.id == fromParams.boxId;
//                    }) || {};
//                    abox.updates = 0;
//                }

//            }
//        });
//        $scope.$watch(function () {
//            if (dashboardService.boxes) {
//                return dashboardService.boxes.length;
//            }
//            return dashboardService.boxes;
//        }, function (val) {
//            if (angular.isNumber(val)) {
//                if (val > 0) {
//                    d.showBoxesNodes = true;
//                } else {
//                    d.showBoxesNodes = false;
//                }
//                return;
//            }
//            if (!val) {
//                //we don't know what is the status
//                d.showBoxesNodes = true;
//            }
//        });
//    }
//})();