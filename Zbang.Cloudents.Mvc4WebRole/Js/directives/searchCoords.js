app.directive('searchCoords',
    [
    function () {
        "use strict";
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                


                var $searchPopup = $('#searchPopup');
                scope.$on('viewContentLoaded', function () {
                    var offset = elem.offset(),
                    width = elem.width(),
                    height = elem.height(),
                    centerX = offset.left + width / 2,
                    centerY = offset.top + height / 2;
                    console.log(centerX, centerY);
                    $searchPopup.css('-webkit-transform-origin', centerX + 'px ' + centerY + 'px');
                });
             
            }
        };
    }
    ]);
