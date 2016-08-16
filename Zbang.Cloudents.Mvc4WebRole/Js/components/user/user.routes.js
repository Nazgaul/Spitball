'use strict';
(function () {
    angular.module('app.user').run(appRun);

    appRun.$inject = ['routerHelper'];
    function appRun(routerHelper) {
        routerHelper.configureStates(getStates());


        function getStates() {
            return [
            {
                state: 'user',
                config: {
                    url: '/user/:userId:int/:userName/',
                    controller: 'UserController as u',
                    resolve: {
                        userData: [
                            'userService', '$stateParams', function (userService, $stateParams) {
                                return userService.getDetails($stateParams.userId);

                            }
                        ]
                    }
                },
                templateUrl: '/user/indexpartial/'

            }
           
            ];
        }
    }
})();