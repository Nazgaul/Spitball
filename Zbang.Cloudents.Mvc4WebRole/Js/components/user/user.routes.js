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
                                profileData: [
                                    "userService", "$stateParams", function (userService, $stateParams) {
                                        return userService.getDetails($stateParams.userId);
                                    }
                                ]
                            }
                        },
                        templateUrl: "/user/indexpartial/"
                    },
                    {
                        state: 'user.badge',
                        config: {
                            url: 'badges/?{type:level|badge|community}',
                            controller: 'gamification as g',
                            parent: 'user',
                            reloadOnSearch: false
                        },
                        templateUrl: "/user/badges/"
                    },
                    {
                        state: 'user.item',
                        config: {
                            url: 'items/',
                            controller: 'item as i',
                            parent: 'user'
                        },
                        templateUrl: '/user/uploads/'
                    }, {
                        state: 'user.quiz',
                        config: {
                            url: 'quizzes/',
                            controller: 'quiz as q',
                            parent: 'user'
                        },
                        templateUrl: '/user/quizzes/'
                    }, {
                        state: 'user.feed',
                        config: {
                            url: 'feed/',
                            controller: 'feed as f',
                            parent: 'user'
                        },
                        templateUrl: '/user/posts/'
                    }, {
                        state: 'user.classmates',
                        config: {
                            url: 'members/',
                            controller: 'classmates as c',
                            parent: 'user'
                        },
                        templateUrl: '/user/classmates/'
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
//# sourceMappingURL=user.routes.js.map