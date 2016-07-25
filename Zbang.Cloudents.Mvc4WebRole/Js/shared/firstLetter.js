'use strict';
(function () {
    angular.module('app').directive('firstLetter',
        function () {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    if (attrs.firstLetter) {
                        element.text(attrs.firstLetter[0]);
                    }
                }
            };
        }
    );
})();
(function () {
    angular.module('app').directive('univeristyCover', univeristyCover);
    univeristyCover.$inject = ['itemThumbnailService'];

    function univeristyCover(itemThumbnailService) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var url = itemThumbnailService
                    .getUniversityPic(attrs.univeristyCover, element.width(), element.height());
                element.css('background', 'url('+url+')');
                //if (attrs.firstLetter) {
                //    element.text(attrs.firstLetter[0]);
                //}
            }
        };
    }

})();




