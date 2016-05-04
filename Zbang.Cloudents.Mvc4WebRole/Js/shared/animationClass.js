'use strict';
(function () {
    angular.module('app').directive('animationClass', animClass);
    animClass.$inject = ['$state'];

    function animClass($state) {
        return {
            link: function(scope, elem) {

                if (scope.app.clickLocation) {
                    //var x = scope.app.clickLocation.x, y = scope.app.clickLocation.y;
                    //elem.css({
                    //    '-moz-transform-origin': x + ' ' + y,
                    //    '-ms-transform-origin': x + ' ' + y,
                    //    '-webkit-transform-origin': x + ' ' + y,
                    //    'transform-origin': x + ' ' + y
                    //
                    //});
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
