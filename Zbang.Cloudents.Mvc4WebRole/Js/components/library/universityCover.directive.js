(function () {
    "use strict";
    angular.module("app").directive("univeristyCover", univeristyCover);
    univeristyCover.$inject = ["itemThumbnailService", "$timeout"];

    function univeristyCover(itemThumbnailService, $timeout) {

        return {
            //scope: {
            //    univeristyCover : '@'
            //},
            restrict: 'A',
            link: function (scope, element, attrs) {
                function appenUrl() {
                    var url = itemThumbnailService
                                .getUniversityPic(attrs.univeristyCover,
                                element.outerWidth(), element.outerHeight());
                    element.css("background", "url(" + url + ')');
                }
                //need because the css height need to kicks in
                $timeout(appenUrl);
            }
        };
    }

})();