(function () {
    angular.module('app').directive('firstLetter',
        function () {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    element.text(attrs.firstLetter[0].toUpperCase());
                }
            };
        }
    );
})();