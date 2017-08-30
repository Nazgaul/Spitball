app.directive('focusOn',
    [

    function () {
        "use strict";
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                scope.$on('focusOn', function (e, name) {
                    if (name === attrs.focusOn) {
                        elem[0].focus();
                    }
                });
            }
        };
    }
    ]);
