(function () {
    angular.module('app').directive('mixitup', ['$timeout', function ($timeout) {
        var linker = function (scope, element, attrs) {

            scope.$on("mixItUp", function () {
                $timeout(function () {
                    $(element).mixitup();
                });
            });
        };
        return {
            restrict: 'A',
            link: linker
        };
    }]);
})();
