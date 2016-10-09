(function () {
    'use strict';

    angular.module('app').directive('keyboardAction', keyboardAction);

    function keyboardAction() {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var keyDownHandler = function (e) {
                    if (e.keyCode == attrs.keyboardAction) {
                        element.click();
                    }
                }

                document.addEventListener('keydown', keyDownHandler);

                scope.$on('$destroy', function () {
                    document.removeEventListener('keydown', keyDownHandler);
                });
            }
        };
    }
})();