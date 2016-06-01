'use strict';
(function () {
    angular.module('app').run(intercom);
    intercom.$inject = ['userDetailsFactory', '$rootScope', '$mdMenu'];
    function intercom(userDetailsFactory, $rootScope, $mdMenu) {
        var started = false;
        function start() {
            if (started) {
                return;
            }
            started = true;
            var data = userDetailsFactory.get();
            if (data.id) {
                Intercom('boot', {
                    app_id: "njmpgayv",
                    name: data.name,
                    email: data.email,
                    created_at: Math.round(data.createTime.getTime() / 1000),
                    user_id: data.id,
                    user_image: data.image,
                    university_id: data.university.id,
                    university_name: data.university.name,
                    reputation: data.score,
                    language: data.culture,
                    university_country: data.university.country,
                    widget: {
                        activator: '#Intercom'
                    }
                });
                Intercom('onActivatorClick', function () {
                    $mdMenu.hide();
                });
            }
            else {
                Intercom('boot', {
                    app_id: "njmpgayv"
                });
            }
        }
        function stop() {
            started = false;
            Intercom('shutdown');
        }
        $rootScope.$on('$stateChangeSuccess', function (event, toState) {
            start();
        });
    }
})();
