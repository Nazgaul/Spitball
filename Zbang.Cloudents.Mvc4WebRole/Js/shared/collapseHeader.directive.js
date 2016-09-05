'use strict';
(function () {
    angular.module('app').directive('collapseHeader', collapseHeader);
    collapseHeader.$inject = ['$window', '$rootScope'];

    function collapseHeader($window, $rootScope) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                //scope.app.collapsedHeader = false;
                var $container = angular.element($window);
                var $body = angular.element('body');
                //$container.on('scroll', onScroll);

                var lastScrollTop = 0;
                $container.on('scroll', onScroll);

                function onScroll(event) {
                    //var scrollPos = $container.scrollTop();
                    //if (scrollPos > 0) {
                    //    scope.app.collapsedHeader = true;
                    //} else {
                    //    scope.app.collapsedHeader = false;
                    //}



                    var st = $container.scrollTop();
                    if (st > lastScrollTop) {
                        $body.addClass('collapseHeader');
                        //alert('scroll down');
                       // scope.app.collapsedHeader = true;
                    } else {
                        $body.removeClass('collapseHeader');
                        //alert('scroll up');
                        //scope.app.collapsedHeader = false;
                    }
                    lastScrollTop = st;


                    //if (event.originalEvent.wheelDelta >= 0) {
                    //    scope.app.collapsedHeader = false;
                    //}
                    //else {
                    //    scope.app.collapsedHeader = true;
                    //}

                }

                scope.$on('$destroy', function () {
                    $body.removeClass('collapseHeader');
                    $container.unbind('scroll', onScroll);
                });

            }
        };
    }
})();