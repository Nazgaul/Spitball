angular.module('app').
    directive('loader', ['$rootScope', function ($rootScope) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                $rootScope.$on('$stateChangeStart', function () {
                    element.css({ display: 'inline-block' });
                });
                $rootScope.$on('$stateLoaded', function () {
                    element.css({ display: 'none' });
                });
            }
        }
    }])