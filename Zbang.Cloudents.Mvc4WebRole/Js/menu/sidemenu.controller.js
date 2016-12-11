var app;
(function (app) {
    'use strict';
    var loaded = false;
    var SideMenu = (function () {
        function SideMenu(user, dashboardService, $location, $scope, $mdSidenav, $state) {
            var _this = this;
            this.user = user;
            this.dashboardService = dashboardService;
            this.$location = $location;
            this.$scope = $scope;
            this.$mdSidenav = $mdSidenav;
            this.$state = $state;
            this.courses = [];
            this.privateBoxes = [];
            this.userUrl = this.$state.href("user", { userId: this.user.id, userName: this.user.name });
            this.showBoxesNodes = true;
            this.coursesOpen = false;
            this.boxesOpen = false;
            loaded = false;
            $scope.$on("close-menu", function () {
                $mdSidenav("left").close();
            });
            $scope.$on("open-menu", function () {
                $mdSidenav("left").toggle();
            });
            $scope.$on("remove-box", function (e, arg) {
                arg = parseInt(arg, 10);
                _this.removeElement(_this.courses, arg);
                _this.removeElement(_this.privateBoxes, arg);
            });
            $scope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
                if (fromState.parent === "box") {
                    if (fromParams.boxtype === "box") {
                        var box = _this.privateBoxes.find(function (i) { return (i.id === fromParams.boxId); }) || {};
                        box.updates = 0;
                    }
                    else {
                        var abox = _this.courses.find(function (i) { return (i.id === fromParams.boxId); }) || {};
                        abox.updates = 0;
                    }
                }
            });
            $scope.$watch(function () {
                if (dashboardService.boxes) {
                    return dashboardService.boxes.length;
                }
                return dashboardService.boxes;
            }, function (val) {
                if (angular.isNumber(val)) {
                    if (val > 0) {
                        _this.showBoxesNodes = true;
                    }
                    else {
                        _this.showBoxesNodes = false;
                    }
                    return;
                }
                if (!val) {
                    _this.showBoxesNodes = true;
                }
            });
        }
        SideMenu.prototype.univeristyClick = function () {
            var _this = this;
            this.dashboardService.getUniversityMeta().then(function (response) {
                _this.$location.path(decodeURIComponent(response.url));
            });
        };
        SideMenu.prototype.initOpen = function () {
            if (!loaded) {
                this.getBoxes();
                loaded = true;
            }
            return true;
        };
        SideMenu.prototype.toggleCourses = function () {
            if (!this.initOpen()) {
                return;
            }
            this.coursesOpen = !this.coursesOpen;
            this.boxesOpen = false;
        };
        SideMenu.prototype.toggleBoxes = function () {
            if (!this.initOpen()) {
                return;
            }
            this.coursesOpen = false;
            this.boxesOpen = !this.boxesOpen;
        };
        SideMenu.prototype.isSectionSelected = function (section) {
            return decodeURI(this.$location.url()).startsWith(section);
        };
        SideMenu.prototype.getBoxes = function () {
            var _this = this;
            this.dashboardService.getBoxes().then(function (response2) {
                for (var i = 0; i < response2.length; i++) {
                    var b = response2[i];
                    if (b.boxType.startsWith('academic')) {
                        _this.courses.push(b);
                    }
                    else {
                        _this.privateBoxes.push(b);
                    }
                }
            });
        };
        SideMenu.prototype.removeElement = function (arr, arg) {
            var box = arr.find(function (e) { return (e.id === arg); });
            if (box) {
                var index = arr.indexOf(box);
                arr.splice(index, 1);
            }
        };
        SideMenu.$inject = ["user", "dashboardService", "$location", "$scope", "$mdSidenav", "$state"];
        return SideMenu;
    }());
    angular.module("app").controller("SideMenu", SideMenu);
})(app || (app = {}));
//# sourceMappingURL=sidemenu.controller.js.map