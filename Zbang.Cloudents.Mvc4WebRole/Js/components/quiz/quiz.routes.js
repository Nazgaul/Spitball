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
                    controller: 'QuizController as q'


                }
            }, {
                state: 'quizCreate',
                config: {
                    templateUrl: function () {
                        return routerHelper.buildUrl('/quiz/createpartial/');
                    },
                    url: '/{boxtype:box|course}/{universityType}/{boxId}/{boxName}/quizcreate/',
                    //controller: 'FeedController as f'
                }
            }


            ];
        }
    }
})();