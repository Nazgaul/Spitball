var app;
(function (app) {
    "use strict";
    var AppRun = (function () {
        function AppRun(routerHelper) {
            this.routerHelper = routerHelper;
            routerHelper.configureStates(getStates());
            function getStates() {
                return [
                    {
                        state: "user",
                        config: {
                            url: "/user/{userId:int}/{userName:encodeStr}/",
                            controller: "UserController as u",
                            resolve: {
                                userData: [
                                    "userService", "$stateParams", function (userService, $stateParams) {
                                        return userService.getDetails($stateParams.userId);
                                    }
                                ]
                            }
                        },
                        templateUrl: "/user/indexpartial/"
                    }
                ];
            }
        }
        AppRun.factory = function () {
            var factory = function (routerHelper) {
                return new AppRun(routerHelper);
            };
            factory["$inject"] = ["routerHelper"];
            return factory;
        };
        return AppRun;
    }());
    angular.module('app.user').run(AppRun.factory());
})(app || (app = {}));
