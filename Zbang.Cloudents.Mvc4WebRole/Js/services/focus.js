define('focus',['app'], function (app) {    
    app.factory('Focus',
        ['$rootScope', '$timeout',
        function ($rootScope, $timeout) {
            return function (name) {
                $timeout(function () {
                    $rootScope.$broadcast('focusOn', name);
                });
            };
        }
    ]);
});

