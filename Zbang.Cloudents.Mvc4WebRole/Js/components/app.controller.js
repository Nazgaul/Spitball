﻿(function () {
    angular.module('app').controller('AppController', appController);
    appController.$inject = ['$rootScope', '$window', '$location', 'history', '$state', 'userDetailsFactory', '$mdToast', '$document'];

    function appController($rootScope, $window, $location, h, $state, userDetails, $mdToast, $document) {
        var self = this;
        $rootScope.$on('$viewContentLoaded', function () {
            var path = $location.path(),
                absUrl = $location.absUrl(),
                virtualUrl = absUrl.substring(absUrl.indexOf(path));
            $window.dataLayer.push({ event: 'virtualPageView', virtualUrl: virtualUrl });
        });

        self.back = function (defaultUrl) {
            var element = h.popElement();
            if (!element) {
                $location.url(defaultUrl);
                return;
            }
            if (typeof element === 'string') {
                $location.url(element);
            }
            $rootScope.$broadcast('from-back');
            //element.params.fromBack = true;
            $state.go(element.name, element.params);
        }

        self.openMenu = openMenu;
        self.hideSearch = false;
        self.resetForm = resetForm;
        self.showToaster = showToaster;

        function showToaster(text,parentId) {
            $mdToast.show(
                  $mdToast.simple()
                  .textContent(text)
                  .position('top')
                  .parent($document[0].querySelector('#' + parentId))
                  .hideDelay(1000));
        }

        // var originatorEv;
        function openMenu($mdOpenMenu, ev) {
            //originatorEv = ev;
            $mdOpenMenu(ev);
        };

        function resetForm(myform) {
            myform.$setPristine();
            myform.$setUntouched();
        }

        $rootScope.$on('$stateChangeSuccess', function (event, toState) {
            var name = toState.name;
            if (name === 'search' || name === 'universityChoose') {
                self.hideSearch = true;
            } else {
                self.hideSearch = false;
            }


        });

        //$rootScope.$on('viewContentLoaded', function() {
        //    svg4everybody();
        //});



        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
            if (fromParams.boxId && toParams.boxId) {
                if (fromParams.boxId === toParams.boxId && toState.name === 'box' && fromState.name.startsWith('box')) {
                    event.preventDefault();
                    $rootScope.$broadcast('state-change-start-prevent');
                }
            }
            $rootScope.$broadcast('close-collapse');
            if (!userDetails.isAuthenticated()) {
                return;
            }
            var details = userDetails.get();
            if (details.university.id) {
                return;
            }
            var userWithNoUniversityState = 'universityChoose';
            if (toState.name !== userWithNoUniversityState) {
                $rootScope.$broadcast('state-change-start-prevent');
                event.preventDefault();
            }

        });



    }
})();