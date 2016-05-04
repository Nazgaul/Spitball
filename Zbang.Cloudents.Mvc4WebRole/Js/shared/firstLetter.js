'use strict';
(function () {
    angular.module('app').directive('firstLetter',
        function () {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    if (attrs.firstLetter) {
                        element.text(attrs.firstLetter[0]);
                    }
                }
            };
        }
    );
})();




