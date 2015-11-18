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
                    url: '/quiz/{universityName}/{boxId}/{boxName}/{quizId}/{quizName}/',
                    templateUrl: function () {
                        return routerHelper.buildUrl('/quiz/indexpartial/');
                    },
                    controller: 'QuizController as q',
                    resolve: {
                        data: ['quizService', '$stateParams', function (quizService, $stateParams) {
                            return quizService.getQuiz($stateParams.boxId, $stateParams.quizId);
                        }]
                    }


                }
            }, {
                state: 'quizCreate',
                config: {
                    templateUrl: function () {
                        return routerHelper.buildUrl('/quiz/createpartial/');
                    },
                    url: '/{boxtype:box|course}/{universityType}/{boxId}/{boxName}/quizcreate/',
                    controller: 'QuizCreateController as q',
                    resolve: {
                        draft: ['quizService', '$location', function (quizService, $location) {
                            if ($location.search().quizid) {
                                return quizService.draft($location.search().quizid);
                            }

                        }],
                        boxUrl: ['$location', function ($location) {
                            var path = $location.path().slice(0, -1),
                                 index = path.lastIndexOf('/');

                            return path.substring(0, index) + '/' + '#quizzes';
                            //$location.path(path).hash('quizzes');
                        }]
                    }
                }
            }


            ];
        }
    }
})();