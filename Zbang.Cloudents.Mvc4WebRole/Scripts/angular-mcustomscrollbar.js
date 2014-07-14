angular.module('custom_scrollbar', []).directive('mCustomScrollbar', ['$parse', function ($parse) {
    var parse = $parse;
    return {
        restrict: 'E',
        transclude: true,
        template: '<div><div ng-transclude></div></div>',
        replace: true,
        link: function ($scope, $elem, $attr) {
            var $win = $(window), height, top, bottom;

            setScroll();

            $scope.$on('update-scroll', updateScroll);

            $win.resize(updateScroll);

            function updateScroll() {
                setTimeout(function () {
                    $elem.mCustomScrollbar('destroy');
                    setScroll();
                }, 50)
            }

            function setScroll() {
                calcHeight();
               
                setTimeout(function () {
                    $scope.$apply(function () {
                        $elem.mCustomScrollbar({ setHeight: height, theme: "dark" });
                    });
                }, 50);
                
            }

            function calcHeight() {
                top = $attr.top ? parseInt($attr.top) : cd.getElementPosition($elem[0]).top;
                bottom = $attr.bottom ? parseInt($attr.bottom) : 0;
                height = $win.height() - (top + bottom);
            }
        }
    };
}]);


