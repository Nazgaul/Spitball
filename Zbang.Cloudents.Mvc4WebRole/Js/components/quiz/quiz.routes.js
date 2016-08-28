﻿'use strict';
(function () {
    angular.module('app.quiz').run(appRun);

    appRun.$inject = ['routerHelper'];
    function appRun(routerHelper) {
        routerHelper.configureStates(getStates());


        function getStates() {
            return [
            {
                state: 'quiz',
                config: {
                    url: '/quiz/{universityName}/{boxId:int}/{boxName}/{quizId:int}/{quizName}/',
                    controller: 'QuizController as q',
                    data: { animateClass: 'quizPage' },
                    resolve: {
                        data: [
                            'quizService', '$stateParams', function(quizService, $stateParams) {
                                return quizService.getQuiz($stateParams.boxId, $stateParams.quizId);
                            }
                        ]
                    },
                    views: {
                        "menu@": {
                            template: ''
                        }
                    }


                },
                templateUrl: '/quiz/indexpartial/'
            }, {
                state: 'quizCreate',
                config: {
                    url: '/{boxtype:box|course}/{universityType}/{boxId:int}/{boxName}/quizcreate/?{quizid:int}&name',
                    controller: 'QuizCreateController as q',
                    resolve: {
                        quizData: [
                            'quizService', '$stateParams', function (quizService, $stateParams) {
                                if ($stateParams.quizid) {
                                    return quizService.draft($stateParams.quizid);
                                }

                            }
                        ],
                        //boxUrl: [
                        //    '$location', function ($location) {
                        //        var path = $location.path().slice(0, -1),
                        //            index = path.lastIndexOf('/');

                        //        return path.substring(0, index) + '/' + 'quizzes';
                        //    }
                        //],
                        //boxName: ['$location', '$stateParams', function ($location, $stateParams) {
                        //    if ($location.search().name) {
                        //        return $location.search().name;
                        //    }
                        //    return $stateParams.boxName;
                        //}],
                        loadMyCtrl: [
                            '$ocLazyLoad', function($ocLazyLoad) {
                                return $ocLazyLoad.load('quizCreate');
                            }
                        ]
                    },
                    data: { animateClass: 'full-screen quiz-create' },
                    reloadOnSearch: false

        },
                templateUrl: '/quiz/createpartial/'
            }


            ];
        }
    }
})();