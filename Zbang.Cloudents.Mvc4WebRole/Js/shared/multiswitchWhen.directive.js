(function () {
    'use strict';
    angular.module('app').directive('multiswitchWhen', function () {
        return {
            transclude: 'element',
            priority: 800,
            require: '^ngSwitch',
            link: function (scope, element, attrs, ctrl, $transclude) {
                var selectTransclude = { transclude: $transclude, element: element };
                angular.forEach(attrs.multiswitchWhen.split('|'), function (switchWhen) {
                    ctrl.cases['!' + switchWhen] = (ctrl.cases['!' + switchWhen] || []);
                    ctrl.cases['!' + switchWhen].push(selectTransclude);
                });
            }
        }
    });
})();