"use strict";
(function () {
    angular.module('app').directive('scrollToggle', scrollToggle);
    scrollToggle.$inject = ['$window', '$rootScope'];
    function scrollToggle($window, $rootScope) {
        return {
            restrict: 'A',
            link: function (scope, element) {
                var lastY;
                $(document).bind('touchmove', function (e) {
                    var currentY = e.originalEvent.touches[0].clientY;
                    if (Math.abs(currentY - lastY) > 150) {
                        if (currentY > lastY) {
                            // moved down
                            element.removeClass('hidden-elemnt');
                        }
                        else if (currentY < lastY) {
                            // moved up
                            element.addClass('hidden-elemnt');
                        }
                    }
                    lastY = currentY;
                });
            }
        };
    }
})();
//# sourceMappingURL=scrollToggle.directive.js.map