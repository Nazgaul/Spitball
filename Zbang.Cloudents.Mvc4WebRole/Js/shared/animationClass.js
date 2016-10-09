(function () {
    'use strict';
    angular.module('app').directive('animationClass', animClass);
    animClass.$inject = ['$state'];

    function animClass($state) {
        return {
            link: function(scope, elem) {
                if (scope.app.clickLocation) {
                    scope.app.clickLocation = null;
                }
                var enterClass;
                if ($state.current.data) {
                    enterClass = $state.current.data.animateClass;
                    elem.addClass(enterClass);
                }
                scope.$on('$destroy', function() {
                    elem.removeClass(enterClass);
                    if ($state.current.data) {
                        elem.addClass($state.current.data.animateClass);
                    }
                });
            }
        };
    }
})();
