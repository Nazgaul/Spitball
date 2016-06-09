'use strict';
(function () {
    'use strict';

    angular.module('app').directive('keyboardAction', keyboardAction);

    function keyboardAction() {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                document.addEventListener('keydown', function (e) {
                    if (e.keyCode == attrs.keyboardAction) {
                        element.click();
                    }
                });
            }
        };
    }
})();