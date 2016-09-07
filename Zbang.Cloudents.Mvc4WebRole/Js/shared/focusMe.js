﻿(function () {
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
                // to address @blesh's comment, set attribute value to 'false'
                // on blur event:
                element.bind('blur', function () {
                    scope.$apply(model.assign(scope, false));
                });
            }
        };
    };
})();
