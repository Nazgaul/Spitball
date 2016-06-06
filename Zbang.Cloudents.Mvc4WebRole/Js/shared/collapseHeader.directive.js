'use strict';
(function () {
    angular.module('app').directive('collapseHeader', collapseHeader);
    collapseHeader.$inject = ['$window'];

    function collapseHeader($window) {
        return {
            restrict: 'a',
            link: function ($window) {
                var $container = angular.element($window);

                $container.on('scroll', $handle);
                scope.$on('$destroy', function () {
                    $container.unbind('scroll', $handle);
                });

                function $handle() {
                    var scrollPos = $container.scrollTop();
                    //var unregContainer = $(".unreg-user .content-wrapper");
                    if (scrollPos > 0) {
                        $('body').addClass('collapse-header');
                    } else {
                        $('body').removeClass('collapse-header');
                    }
                }

            }
        };
    }
})();