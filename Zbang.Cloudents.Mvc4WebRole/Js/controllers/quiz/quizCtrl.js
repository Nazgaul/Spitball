define('QuizCtrl', ['app'], function (app) {
    app.controller('QuizCtrl',
        ['$scope',
        function ($scope) {
            cd.pubsub.publish('initQuizView');
            cd.pubsub.publish('quiz');

            //todo proper return;
        }
        ]);
});