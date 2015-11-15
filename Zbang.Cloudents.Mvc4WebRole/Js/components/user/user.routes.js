﻿(function () {
    angular.module('app.user').run(appRun);

    appRun.$inject = ['routerHelper'];
    function appRun(routerHelper) {
        routerHelper.configureStates(getStates());


        function getStates() {
            return [
            {
                state: 'user',
                config: {
                    url: '/user/:userId/:userName/',
                    templateUrl: function () {
                        return routerHelper.buildUrl('/user/indexpartial/');
                    },
                    controller: 'UserController as u',
                    resolve: {
                        userData: [
                            'userService', '$stateParams', function (userService, $stateParams) {
                                return userService.getDetails($stateParams.userId);

                            }
                        ]
                    }
                },

            },
            //TODO: this is ugly
             {
                 state: 'userhash',
                 config: {
                     url: '/user/:userId/:userName/#{state}',
                     templateUrl: function () {
                         return routerHelper.buildUrl('/user/indexpartial/');
                     },
                     controller: 'UserController as u',
                     resolve: {
                         userData: [
                             'userService', '$stateParams', function (userService, $stateParams) {
                                 return userService.getDetails($stateParams.userId);

                             }
                         ]
                     }
                 },

             }
            ];
        }
    }
})();