(function() {
    angular.module('app.box.feed').directive('slideit', slideit);
    slideit.$inject = ['$timeout'];
    function slideit($timeout) {
        return {
            restrict: 'A',
            //replace: true,
            //scope: {
            //    slideit: '='
            //},
            //template: '<ul>' +
            //    '<li ng-repeat="e in slides" ng-include="f.postItemTemplate(e)">' +
            //    '</li>' +
            //    '</ul>',
            link: function(scope, elm, attrs) {
                //var x = attrs.slideit;
                //elm.ready(function () {
                //scope.$apply(function () {
                //scope.slides = scope.slideit;
                //});
                $timeout(function () {
                    elm.bxSlider({
                        adaptiveHeight: true,
                        infiniteLoop: false,
                        slideWidth: 119,
                        minSlides: 2,
                        maxSlides: 3,
                        pager: false,
                        slideMargin: 10,
                        hideControlOnEnd: true,
                        mode: 'horizontal'
                    });
                },1);
                //});
            }
        }
    }
})();
(function() {
    angular.module('app.box.feed').directive('bxSliderItem', bxSliderItem);

    function bxSliderItem() {
        return {
            require: '^bxSlider',
            link: function(scope, elm, attr, bxSliderCtrl) {
                if (scope.$last) {
                    bxSliderCtrl.update();
                }
            }
        }
    }
})();
(function () {
    angular.module('app.box.feed').directive('bxSlider', bxSlider);
    var BX_SLIDER_OPTIONS = {
        adaptiveHeight: true,
        infiniteLoop: false,
        slideWidth: 119,
        minSlides: 2,
        maxSlides: 5,
        pager: false,
        slideMargin: 10,
        hideControlOnEnd: true,
        mode: 'horizontal'
    };
    function bxSlider() {
        return {
            restrict: 'A',
            require: 'bxSlider',
            priority: 0,
            controller: function () { },
            link: function (scope, element, attrs, ctrl) {
                var slider;
                ctrl.update = function () {
                    slider && slider.destroySlider();
                    slider = element.bxSlider(BX_SLIDER_OPTIONS);
                };
            }
        }
    }
})();