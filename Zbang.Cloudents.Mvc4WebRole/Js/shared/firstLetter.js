(function () {
    'use strict';
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
(function() {
    angular.module('app').directive('ngSwitchMultipleWhen', function () {
        // Exact same definition as ngSwitchWhen except for the link fn
        return {
            // Same as ngSwitchWhen
            priority: 1200,
            transclude: 'element',
            require: '^ngSwitch',
            link: function (scope, element, attrs, ctrl, $transclude) {
                var caseStms = scope.$eval(attrs.ngSwitchMultipleWhen);
                caseStms = angular.isArray(caseStms) ? caseStms : [caseStms];

                angular.forEach(caseStms, function (caseStm) {
                    caseStm = '!' + caseStm;
                    ctrl.cases[caseStm] = ctrl.cases[caseStm] || [];
                    ctrl.cases[caseStm].push({ transclude: $transclude, element: element });
                });
            }
        };
    });
})();





