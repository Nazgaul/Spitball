(function () {
    'use strict';
    angular.module('app').directive('focusMe', focusMe);
    focusMe.$inject = ['$timeout', '$parse'];
    function focusMe($timeout, $parse) {
        return {
            //scope: true,   // optionally create a child scope
            link: function (scope, element, attrs) {
                var model = $parse(attrs.focusMe);

                scope.$watch(model, function (value) {
                    if (value === true) {
                        $timeout(function () {
                            element[0].focus();
                        });
                    }
                });
                element.bind('blur', function () {
                    scope.$apply(model.assign(scope, false));
                });
            }
        };
    };
})();

(function () {
    //TODO different file
    // in order to raise keyboard we need user interaction in ios
    'use strict';
    angular.module('app').directive('focusUponClick', focusMe);
    //focusMe.$inject = ['$timeout', '$parse'];
    function focusMe() {
        return {
            //scope: true,   // optionally create a child scope
            link: function (scope, element, attrs) {
                element.click(function () {
                    $('#' + attrs.focusUponClick).focus();
                });
            }
        };
    };
})();
