'use strict';
(function () {
    'use strict';

    angular.module('app').directive('keyboardAction', keyboardAction);

    function keyboardAction() {
        document.onkeydown = function (e) {
            if (e.keyCode == e.target.attributes.keyboardAction) {
                element.click();
                console.log(e.which);
            }
        };
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                //$(document).bind('onkeydown', function () {
                //    console.log('xxx')
                //})
                
                
                //document.onkeydown = function (e) {
                //    if (e.keyCode == attrs.keyboardAction) {
                //        element.click();
                //        console.log(e.which);
                //    }
                //};
            }
        };
    }
})();