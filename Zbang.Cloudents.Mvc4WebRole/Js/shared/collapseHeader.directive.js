(function () {
    'use strict';
    angular.module('app').directive('collapseHeader', collapseHeader);
    collapseHeader.$inject = ['$window', '$timeout'];
    //TODO: on item directory
    function collapseHeader($window, $timeout) {
        return {
            restrict: 'A',
            link: function (scope) {
                var $container = angular.element($window);
                var $body = angular.element('body');
                var className = 'collapseHeader';
                var lastScrollTop = 0;
                var promise;
                $container.on('scroll', onScroll);
                $container.on('touchmove', onScroll);



                function onScroll() {
                    $timeout.cancel(promise);
                    promise = $timeout(function () {
                        var st = Math.max($body.scrollTop(), 0);
                        //st = Math.min($body.scrollTop(), 0);
                        if (st === lastScrollTop) {
                            return;
                        }
                        if ((window.innerHeight + window.scrollY) + 100 >= document.body.offsetHeight) {
                            return;
                        }
                        if (st > lastScrollTop) {
                            $body.addClass(className);
                        } else {
                            $body.removeClass(className);
                        }
                        lastScrollTop = st;

                    }, 10);
                }

                scope.$on('$destroy', function () {
                    $body.removeClass(className);
                    $container.unbind('scroll', onScroll);
                    $container.unbind('touchmove', onScroll);
                });

            }
        };
    }
})();