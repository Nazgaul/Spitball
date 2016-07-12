'use strict';
(function () {
    angular.module('app.dashboard').controller('SideMenu', sideMenu);
    sideMenu.$inject = ['dashboardService', 'userDetailsFactory', '$rootScope', '$mdSidenav', '$location', '$timeout', '$scope'];

    function sideMenu(dashboardService, userDetails, $rootScope, $mdSidenav, $location, $timeout, $scope) {
        var d = this, loaded = false;
        d.courses = [];
        d.privateBoxes = [];

        userDetails.init().then(function (userDetails) {
            d.userUrl = userDetails.url;
            if (userDetails.theme === 'dark') {

                d.updateScrollbar = {
                    theme: 'light-thin',
                    scrollbarPosition: 'outside'

                };
            }
        });
        d.showBoxesNodes = true;
        $scope.$watch(function () {
            if (dashboardService.boxes) {
                return dashboardService.boxes.length;
            }
            return dashboardService.boxes;
        }, function (val) {
            if (angular.isNumber(val)) {
                if (val > 0) {
                    d.showBoxesNodes = true;
                } else {
                    d.showBoxesNodes = false;
                }
                return;
            }
            if (!val) {
                //we don't know what is the status
                d.showBoxesNodes = true;
            }
        });

        d.coursesOpen = false;
        d.boxesOpen = false;

        d.isSectionSelected = isSectionSelected;

        d.toggleCourses = toggleCourses;
        d.toggleBoxes = toggleBoxes;


        function toggleCourses() {
            if (!initOpen()) {
                return;
            }
            d.coursesOpen = !d.coursesOpen;
            d.boxesOpen = false;
            $timeout(function () {
                $rootScope.$broadcast('updateScroll');
            }, 800);
        }
        function toggleBoxes() {
            if (!initOpen()) {
                return;
            }
            d.coursesOpen = false;
            d.boxesOpen = !d.boxesOpen;
            $timeout(function () {
                $rootScope.$broadcast('updateScroll');
            }, 800);
        }
        function initOpen() {
            if (!userDetails.isAuthenticated()) {
                $rootScope.$broadcast('show-unregisterd-box');
                return false;
            }
            if (!loaded) {
                getBoxes();
                loaded = true;
            }
            return true;
        }

        function isSectionSelected(section) {
            return $location.url().startsWith(section);
        }

        function getBoxes() {
            dashboardService.getBoxes().then(function (response2) {
                for (var i = 0; i < response2.length; i++) {
                    var b = response2[i];
                    if (b.boxType.startsWith('academic')) {
                        d.courses.push(b);
                    } else {
                        d.privateBoxes.push(b);
                    }
                }

            });
        }

        $scope.$on('close-menu', function () {
            $mdSidenav('left').close();
            $scope.app.menuOpened = false;
            self.menuOpened = !self.menuOpened;
        });
        $scope.$on('open-menu', function () {
            $mdSidenav('left').toggle();
            $scope.app.menuOpened = !$scope.app.menuOpened;
            $timeout(function () {
                $rootScope.$broadcast('updateScroll');
            });
        });
        $scope.$on('remove-box', function (e, arg) {
            arg = parseInt(arg, 10);
            removeElement(d.courses, arg);
            removeElement(d.privateBoxes, arg);
        });

        function removeElement(arr, arg) {
            var box = arr.find(function (e) {
                return e.id === arg;
            });
            if (box) {
                var index = arr.indexOf(box);
                arr.splice(index, 1);
            }
        }

        $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
            if (fromState.parent === 'box') {
                if (fromParams.boxtype === 'box') {
                    var box = d.privateBoxes.find(function (i) {
                        return i.id == fromParams.boxId;
                    }) || {};
                    box.updates = 0;
                } else {
                    var abox = d.courses.find(function (i) {
                        return i.id == fromParams.boxId;
                    }) || {};
                    abox.updates = 0;
                }

            }
        });
    }
})();