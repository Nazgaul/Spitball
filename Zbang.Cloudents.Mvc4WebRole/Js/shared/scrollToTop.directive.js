'use strict';
(function () {
    'use strict';

    angular.module('app').directive('scrollToTop', scrollToTop);

    function scrollToTop() {
        return {
            restrict: 'E',
            replace: true,

            templateUrl: 'scrollToTopTemplate.html',
            link: function ($scope, $element) {
                var offset = 300, duration = 500;

                if (navigator.userAgent.match(/iPhone|iPad|iPod/i)) {  // ios supported
                    $(window).bind("touchend touchcancel touchleave", function () {
                        if ($(this).scrollTop() > offset) {
                            $element.fadeIn(duration);
                        } else {
                            $element.fadeOut(duration);
                        }
                    });
                } else {  // general 
                    $(window).scroll(function () {
                        if ($(this).scrollTop() > offset) {
                            $element.fadeIn(duration);
                        } else {
                            $element.fadeOut(duration);
                        }
                    });
                }

                $element.on('click', function () {
                    $('html, body').animate({ scrollTop: 0 }, duration);
                    return false;
                });

            }
        };
    }
})();

(function () {
    'use strict';

    angular.module('app').directive('scrollToTop', scrollToTop);

    function scrollToTop() {
        return {
            restrict: 'A',
            link: function ($scope, $element) {
                $element.on('click', function () {
                    $('html, body').animate({ scrollTop: 0 }, 0);
                    return false;
                });
            }
        };
    }
})();