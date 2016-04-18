(function () {
    'use strict';

    angular.module('app').directive('shareBox', shareBox);
    shareBox.$inject = ['$window', '$rootScope'];

    function shareBox($window, $rootScope) {
        return {
            restrict: 'E',
            replace: true,
            
            templateUrl: 'shareBoxTemplate.html',
            link: function ($scope, $element) {
                var shareFb = 'https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(window.location);
                $element.find('.share-fb button').on('click', function () {
                    window.open(shareFb, "pop", "width=600, height=400, scrollbars=no");
                });
                $element.find('.share-whatsapp a').attr('href', 'whatsapp://send?text=' + window.location);
            }
        };
    }
})();