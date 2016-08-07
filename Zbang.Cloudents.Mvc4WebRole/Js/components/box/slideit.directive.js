'use strict';
(function () {
    angular.module('app.box.feed').directive('maxElement', maxElement);
    function maxElement() {
        //var elementWidth = 119;
        return {
            scope: {
                maxElement: '='
            },
            //require: '^bxSlider',
            link: function (scope, elm) {
                var elementWidth = 129;//elm.children().first().outerWidth(true);
                //console.log(scope);
                var numberOfElements = parseInt(elm.width() / elementWidth, 10);
                console.log(numberOfElements, elementWidth, elm.width());
                //var post = attr.bxSlider;
                // console.log(attr.bxSlider);
                if (numberOfElements >= scope.maxElement.files.length) {
                    return;
                }

                scope.maxElement.needMore = scope.maxElement.files.length - numberOfElements;
                scope.maxElement.limitFiles = numberOfElements -1;
                //scope.$apply(function() {

                //});
                //console.log(post);

                //if (scope.$last && scope.$index > 1) {

                //    bxSliderCtrl.update();
                //}
            }
        }
    }
})();
//(function () {
//    angular.module('app.box.feed').directive('bxSlider', bxSlider);
//    bxSlider.$inject = ['$timeout'];
//    function bxSlider($timeout) {
//        return {
//            restrict: 'A',
//            require: 'bxSlider',
//            priority: 0,
//            controller: function () { },
//            link: function (scope, element, attrs, ctrl) {
//                var slider, parent = element.parent();


//                ctrl.update = function () {
//                    $timeout(function () {
//                        var BX_SLIDER_OPTIONS = {
//                            infiniteLoop: false,
//                            slideWidth: 119,
//                            maxSlides: 5,
//                            pager: false,
//                            nextSelector: parent.next(),
//                            prevSelector: parent.prev(),
//                            slideMargin: 10,
//                            hideControlOnEnd: true,
//                            mode: 'horizontal'
//                        };
//                        slider && slider.destroySlider();
//                        slider = element.bxSlider(BX_SLIDER_OPTIONS);
//                    });
//                };
//            }
//        }
//    }
//})();