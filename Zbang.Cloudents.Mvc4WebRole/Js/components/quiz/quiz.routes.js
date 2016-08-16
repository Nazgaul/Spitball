'use strict';
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
                            'quizService', '$stateParams', function (quizService, $stateParams) {
                                return quizService.getQuiz($stateParams.boxId, $stateParams.quizId);
                            }
                        ]
                    }


                },
                templateUrl: '/quiz/indexpartial/'
            }, {
                state: 'quizCreate',
                config: {
                    url: '/{boxtype:box|course}/{universityType}/{boxId:int}/{boxName}/quizcreate/?quizid&name',
                    controller: 'QuizCreateController as q',
                    resolve: {
                        draft: [
                            'quizService', '$location', function (quizService, $location) {
                                if ($location.search().quizid) {
                                    return quizService.draft($location.search().quizid);
                                }

                            }
                        ],
                        boxUrl: [
                            '$location', function ($location) {
                                var path = $location.path().slice(0, -1),
                                    index = path.lastIndexOf('/');

                                return path.substring(0, index) + '/' + '#quizzes';
                            }
                        ],
                        boxName: ['$location', '$stateParams', function ($location, $stateParams) {
                            if ($location.search().name) {
                                return $location.search().name;
                            }
                            return $stateParams.boxName;
                        }]
                    },
                    data: { animateClass: 'full-screen quiz-create' },
                    params: { boxName: null }
                },
                templateUrl: '/quiz/createpartial/'
            }


            ];
        }
    }
})();