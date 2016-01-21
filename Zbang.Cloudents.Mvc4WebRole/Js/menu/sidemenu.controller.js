(function () {
    angular.module('app.dashboard').controller('SideMenu', dashboard);
    dashboard.$inject = ['dashboardService', 'userDetailsFactory', '$rootScope', '$mdSidenav', '$location', '$timeout'];

    function dashboard(dashboardService, userDetails, $rootScope, $mdSidenav, $location, $timeout) {
        var d = this, notloaded = true;
        d.courses = [];
        d.privateBoxes = [];

        userDetails.init().then(function () {
            d.userUrl = userDetails.get().url;
        });

        d.coursesOpen = false;
        d.boxesOpen = false;

        //d.isOpen = isOpen;
        //d.toggleOpen = toggleOpen;
        d.isSectionSelected = isSectionSelected;
        //var openedSection;

        d.toggleCourses = toggleCourses;
        d.toggleBoxes = toggleBoxes;


        function toggleCourses() {
            initOpen();
            d.coursesOpen = !d.coursesOpen;
            d.boxesOpen = false;
            $timeout(function() {
                $rootScope.$broadcast('updateScroll');
            }, 1000);
        }
        function toggleBoxes() {
            initOpen();
            d.coursesOpen = false;
            d.boxesOpen = !d.boxesOpen;
            $timeout(function () {
                $rootScope.$broadcast('updateScroll');
            }, 1000);
        }
        function initOpen() {
            if (!userDetails.isAuthenticated()) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            if (notloaded) {
                getBoxes();
                notloaded = false;
            }
        }
        //function isOpen(section) {
        //    return openedSection === section;
        //}

        //function toggleOpen(section) {
        //    if (section === openedSection) {
        //        openedSection = null;
        //        return;
        //    }
        //    openedSection = section;
        //    if (!userDetails.isAuthenticated()) {
        //        $rootScope.$broadcast('show-unregisterd-box');
        //        return;
        //    }
        //    if (notloaded) {
        //        getBoxes();
        //        notloaded = false;
        //    }
        //}

        function isSectionSelected(section) {
            return $location.url().startsWith(section);
        }

        function getBoxes() {
            dashboardService.getBoxes().then(function (response2) {
                for (var i = 0; i < response2.length; i++) {
                    var b = response2[i];
                    if (b.boxType === 'academic') {
                        d.courses.push(b);
                    } else {
                        d.privateBoxes.push(b);
                    }
                }

            });
        }

        $rootScope.$on('open-menu', function () {
            $mdSidenav('left').toggle();
        });
        $rootScope.$on('remove-box', function (e, arg) {
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