(function () {
    angular.module('app').controller('AppController', appController);
    appController.$inject = ['$rootScope', '$window', '$location', 'history', '$state', 'userDetails'];

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

        self.hideSearch = false;
        $rootScope.$on('$stateChangeSuccess', function (event, toState) {
            if (toState.name === 'search') {
                self.hideSearch = true;
            } else {
                self.hideSearch = false;
            }

        });



        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState) {
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

            //if (self.userDetails) {
            //    if (self.userDetails.university.id) {
            //        return;
            //    } else {
            //        if (fromState.name === userWithNoUniversityState && toState.name !== userWithNoUniversityState) {
            //            $rootScope.$broadcast('state-change-start-prevent');
            //            event.preventDefault();
            //            return;
            //        }
            //        if (toState.name !== userWithNoUniversityState) {
            //            $state.go(userWithNoUniversityState);
            //        }

            //        return;
            //    }
            //    return;
            //}
            //event.preventDefault();



        });



    }
})();