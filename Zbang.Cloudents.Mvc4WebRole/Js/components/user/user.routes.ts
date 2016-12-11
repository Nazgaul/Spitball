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
                            //abstract: true,
                            url: "/user/{userId:int}/{userName:encodeStr}/",
                            controller: "UserController as u",
                            resolve: {
                                profileData: [
                                    "userService", "$stateParams", (userService, $stateParams) =>
                                        userService.getDetails($stateParams.userId)
                                ]

                            }
                        },
                        templateUrl: "/user/indexpartial/"

                    },
                    {
                        state: 'user.badge',
                        config: {
                            //?{type:level|badge|community}
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
                    },{
                        state: 'user.flashcard',
                        config: {
                            url: 'flashcards/',
                            controller: 'userFlashcard as f',
                            parent: 'user'
                        },
                        templateUrl: '/user/flashcards/'
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