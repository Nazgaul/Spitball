(function () {
    'use strict';
    angular.module('app').directive('collapseHeader', collapseHeader);
    collapseHeader.$inject = ['$window'];
    //TODO: on item directory
    function collapseHeader($window) {
        return {
            restrict: 'A',
            link: function (scope) {
                var $container = angular.element($window);
                var $body = angular.element('body');
                var className = 'collapseHeader';
                var lastScrollTop = 0;
                $container.on('scroll', onScroll);


                //var timeout;
                //$(window).resize(function () {
                //    clearTimeout(timeout);
                //    timeout = setTimeout(setBackground, 1000);
                //});
                //TODO: scroll is happen allot
                function onScroll() {
                    var st = $container.scrollTop();
                    if (st > lastScrollTop) {
                        $body.addClass(className);
                    } else {
                        $body.removeClass(className);
                    }
                    lastScrollTop = st;
                }

                scope.$on('$destroy', function () {
                    $body.removeClass(className);
                    $container.unbind('scroll', onScroll);
                });

            }
        };
    }
})();