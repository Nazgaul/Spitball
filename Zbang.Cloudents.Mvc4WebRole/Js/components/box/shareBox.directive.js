(function () {
    'use strict';

    angular.module('app').directive('shareBox', shareBox);
    shareBox.$inject = ['ajaxService'];

    function shareBox(ajaxService) {
        return {
            restrict: 'E',
            replace: true,
            
            templateUrl: 'shareBoxTemplate.html',
            link: function ($scope, $element) {
                var shareFb = 'https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(window.location);
                $element.find('button.share-fb').on('click', function () {
                    window.open(shareFb, "pop", "width=600, height=400, scrollbars=no");
                    ajaxService.post('/share/');
                });
                $element.find('a.share-whatsapp').attr('href', 'whatsapp://send?text=' + window.location).on('click', function () {
                    ajaxService.post('/share/');
                });
                
            }
        };
    }
})();