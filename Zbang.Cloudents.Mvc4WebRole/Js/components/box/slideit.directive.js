(function () {
    'use strict';
    angular.module('app.box.feed').directive('maxElement', maxElement);
    function maxElement() {
        //var elementWidth = 119;
        return {
            scope: {
                maxElement: '='
            },
            link: function (scope, elm) {
                var elementWidth = 129;//elm.children().first().outerWidth(true);
                var numberOfElements = parseInt(elm.width() / elementWidth, 10);
                //console.log(numberOfElements, elementWidth, elm.width());
                if (numberOfElements >= scope.maxElement.files.length) {
                    
                    return;
                }

                scope.maxElement.needMore = scope.maxElement.files.length - numberOfElements;
                scope.maxElement.limitFiles = numberOfElements - 1;

            }
        }
    }
})();
