(function () {
    'use strict';

    angular.module('app').directive('keyboardAction', keyboardAction);

    function keyboardAction() {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var keyDownHandler = function (e) {
                    var keys = attrs.keyboardAction.split(" ");
                    if (!element.closest(".ng-hide").length && keys.indexOf(e.keyCode.toString()) != -1) {
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