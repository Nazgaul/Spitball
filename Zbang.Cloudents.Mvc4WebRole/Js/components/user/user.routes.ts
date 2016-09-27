module app {
    "use strict";
    class AppRun {
        constructor(private routerHelper: IRouterHelper) {
            routerHelper.configureStates(getStates());
            function getStates() {
                return [
                    {
                        state: "user",
                        config: {
                            url: "/user/{userId:int}/:userName/",
                            controller: "UserController as u",
                            resolve: {
                                userData: [
                                    "userService", "$stateParams", (userService, $stateParams) => userService.getDetails($stateParams.userId)
                                ]
                            }
                        },
                        templateUrl: "/user/indexpartial/"

                    }
                ];
            }
        }

        static factory() {
            const factory = (routerHelper) => {
                return new AppRun(routerHelper);
            };

            factory["$inject"] = ["routerHelper"];
            return factory;
        }

    }
    angular.module('app.user').run(AppRun.factory());
}