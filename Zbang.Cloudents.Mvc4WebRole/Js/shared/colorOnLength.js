(function() {
    angular.module('app').directive('dColor',
        function() {
            return {
                restrict: 'A',
                link: function(scope, element, attrs) {
                    var length = attrs.dColor % 17;
                    element.addClass('color' + length);
                }
            };
        }
    );
})();


(function() {
    angular.module('app').directive('mixitup', ['$timeout', function ($timeout) {
        var linker = function (scope, element, attrs) {

            scope.$on("boxItemsLoaded", function () {
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
