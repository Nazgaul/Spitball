﻿'use strict';
(function () {
    angular.module('app.box.feed').directive('bxSliderItem', bxSliderItem);

    function bxSliderItem() {
        return {
            require: '^bxSlider',
            link: function (scope, elm, attr, bxSliderCtrl) {
                if (scope.$last && scope.$index > 2) {

                    bxSliderCtrl.update();
                }
            }
        }
    }
})();
(function () {
    angular.module('app.box.feed').directive('bxSlider', bxSlider);
    bxSlider.$inject = ['$timeout'];
    function bxSlider($timeout) {
        return {
            restrict: 'A',
            require: 'bxSlider',
            priority: 0,
            controller: function () { },
            link: function (scope, element, attrs, ctrl) {
                var slider, parent = element.parent();

                var BX_SLIDER_OPTIONS = {
                    //adaptiveHeight: true,
                    infiniteLoop: false,
                    slideWidth: 119,
                    //minSlides: 2,
                    maxSlides: 5,
                    pager: false,
                    // controls: false,
                    nextSelector: parent.next(),
                    prevSelector: parent.prev(),
                    slideMargin: 10,
                    hideControlOnEnd: true,
                    mode: 'horizontal'
                };
                ctrl.update = function () {
                    $timeout(function () {
                        slider && slider.destroySlider();
                        slider = element.bxSlider(BX_SLIDER_OPTIONS);
                    })
                };
            }
        }
    }
})();