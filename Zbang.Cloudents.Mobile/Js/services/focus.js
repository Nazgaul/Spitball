app.factory('sFocus',
    ['$rootScope', '$timeout',
    function ($rootScope, $timeout) {
        "use strict";
        return function (name) {
            $timeout(function () {
                $rootScope.$broadcast('focusOn', name);
            });
        };
    }
    ]);

