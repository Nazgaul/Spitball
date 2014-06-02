angular.module('custom_scrollbar', []).directive('mCustomScrollbar', ['$parse', function ($parse) {
    return {
        restrict: 'E',
        transclude: true,
        template: '<div><div ng-transclude></div></div>',
        replace: true,
        link: function ($scope, $elem, $attr) {
            var height = $(window).height() - cd.getElementPosition($elem[0]).top-100;
            $elem.mCustomScrollbar({ setHeight: height, theme: "dark-thin" });

            $scope.$on('update-scroll', function () {
                $elem.mCustomScrollbar('update');
            });
        }
    };
}]);