/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../shared/userDetails.ts" />
(function () {
    angular.module('app').run(intercom);
    intercom.$inject = ['userDetailsFactory', '$rootScope'];
    function intercom(userDetailsFactory, $rootScope) {
        function start() {
            var data = userDetailsFactory.get();
            var dateCreate = new Date(data.dateTime);
            if (data.id) {
                Intercom('boot', {
                    app_id: "njmpgayv",
                    name: data.name,
                    email: data.email,
                    created_at: Math.round(dateCreate.getTime() / 1000),
                    user_id: data.id,
                    user_image: data.image,
                    university_id: data.universityId,
                    university_name: data.universityName,
                    reputation: data.score,
                    language: data.culture,
                    university_country: data.universityCountry
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
//# sourceMappingURL=intercom.js.map