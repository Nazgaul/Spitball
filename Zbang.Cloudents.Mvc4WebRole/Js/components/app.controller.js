(function () {
    angular.module('app').controller('AppController', appController);
    appController.$inject = ['$rootScope', '$window', '$location', 'history', '$state', 'userDetailsFactory'];

    function appController($rootScope, $window, $location, history, $state, userDetails) {
        var self = this;
        $rootScope.$on('$viewContentLoaded', function () {
            var path = $location.path(),
                absUrl = $location.absUrl(),
                virtualUrl = absUrl.substring(absUrl.indexOf(path));
            $window.dataLayer.push({ event: 'virtualPageView', virtualUrl: virtualUrl });
        });

        self.back = function (defaultUrl) {
            if (history.arr.length === 1) {
                $location.url(defaultUrl);
                return;
            }
            var element = history.arr[history.arr.length - 1];
            $state.go(element.name, element.params);
        }

        self.openMenu = openMenu;
        self.hideSearch = false;


       // var originatorEv;
        function openMenu($mdOpenMenu, ev) {
            //originatorEv = ev;
            $mdOpenMenu(ev);
        };

        
        $rootScope.$on('$stateChangeSuccess', function (event, toState) {
            if (toState.name === 'search') {
                self.hideSearch = true;
            } else {
                self.hideSearch = false;
            }

        });



        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
            if (fromParams.boxId && toParams.boxId) {
                if (fromParams.boxId === toParams.boxId && toState.name === 'box' && fromState.name.startsWidth('box')) {
                    event.preventDefault();
                    $rootScope.$broadcast('state-change-start-prevent');
                }
            }
            var details = userDetails.get();
            if (!details) {
                return;
            }
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