(function () {
    angular.module('app').directive('univeristyCover', univeristyCover);
    univeristyCover.$inject = ['itemThumbnailService', '$timeout'];

    function univeristyCover(itemThumbnailService, $timeout) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                //need because the css height need to kicks in
                $timeout(function() {
                    var url = itemThumbnailService
                        .getUniversityPic(attrs.univeristyCover, element.outerWidth(), element.outerHeight());
                    element.css('background', 'url(' + url + ')');
                });
                //if (attrs.firstLetter) {
                //    element.text(attrs.firstLetter[0]);
                //}
            }
        };
    }

})();