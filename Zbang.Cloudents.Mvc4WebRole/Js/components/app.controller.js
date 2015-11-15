(function () {
    angular.module('app').controller('AppController', appController);
    appController.$inject = ['$rootScope', '$window', '$location', 'history', '$state'];

    function appController($rootScope, $window, $location, history, $state) {
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
        $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
            if (toState.name === 'search') {
                self.hideSearch = true;
            } else {
                self.hideSearch = false;
            }
           
        });

    }
})();