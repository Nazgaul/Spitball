(function () {
    angular.module('app').run(intercom);
    intercom.$inject = ['userDetailsFactory', '$rootScope'];
    function intercom(userDetailsFactory, $rootScope) {
        function start() {
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
                    university_country: data.university.country
                });
            }
            else {
                Intercom('boot', {
                    app_id: "njmpgayv"
                });
            }
        }
        function stop() {
            Intercom('shutdown');
        }
        $rootScope.$on('$stateChangeSuccess', function (event, toState) {
            if (toState.name === 'dashboard') {
                start();
            }
            else {
                stop();
            }
        });
    }
})();
