'use strict';
(function () {
    angular.module('app.box').directive('addToAny', addToAny);
    addToAny.$inject = ['$templateCache', '$timeout', '$compile', '$location'];
    function addToAny($templateCache, $timeout, $compile, $location) {
        return {
            restrict: 'A',
            link: function (scope, element) {
                var template = $templateCache.get('addToAnyTemplate.html');
                scope.url = $location.absUrl();
                element.append(template);
                $compile(element.contents())(scope);
                $timeout(function () {
                    a2a.init('page');
                });
                scope.$on('$stateChangeSuccess', function () {
                    element.empty();
                });
            }
        };
    }
})();
