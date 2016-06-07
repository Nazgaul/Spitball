'use strict';
(function () {
    angular.module('app').directive('collapseHeader', collapseHeader);
    collapseHeader.$inject = ['$window', '$rootScope'];

    function collapseHeader($window, $rootScope) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                scope.app.collapsedHeader = false;
                var $container = angular.element($window);

                //$container.on('scroll', onScroll);

                $container.on('mousewheel', onScroll);

                function onScroll(event) {
                    //var scrollPos = $container.scrollTop();
                    //if (scrollPos > 0) {
                    //    scope.app.collapsedHeader = true;
                    //} else {
                    //    scope.app.collapsedHeader = false;
                    //}


                    if (event.originalEvent.wheelDelta >= 0) {
                        scope.app.collapsedHeader = false;
                    }
                    else {
                        scope.app.collapsedHeader = true;
                    }

                }

            }
        };
    }
})();