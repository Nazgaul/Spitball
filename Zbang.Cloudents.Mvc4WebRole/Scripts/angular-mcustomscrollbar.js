﻿angular.module('custom_scrollbar', []).directive('mCustomScrollbar', ['$timeout', function ($timeout) {    
    return {
        restrict: 'A',
        transclude: true,
        template: '<div><div ng-transclude></div></div>',
        replace: true,
        link: function ($scope, $elem, $attr) {
            var $win = $(window), height, top, bottom;

            $timeout(setScroll, 100);

            $scope.$on('update-scroll', updateScroll);

            $win.resize(updateScroll);

            function updateScroll() {
                $timeout(function () {
                    $elem.mCustomScrollbar('destroy');
                    setScroll();
                }, 50)
            }

            function setScroll() {
                calcHeight();
              
                $elem.mCustomScrollbar({ setHeight: height, theme: "dark", advanded: { updateOnContentResize: false } });
                
            }

            function calcHeight() {
                top = $attr.top ? parseInt($attr.top) : cd.getElementPosition($elem[0]).top;
                bottom = $attr.bottom ? parseInt($attr.bottom) : 0;
                height = $win.height() - (top + bottom);
            }

            $scope.$on('$destroy', function () {
                $elem.mCustomScrollbar('destroy');
            });
            $scope.$on('$routeChangeStart', function () {
                $elem.mCustomScrollbar('destroy');
            });
        }
    };
}]);


