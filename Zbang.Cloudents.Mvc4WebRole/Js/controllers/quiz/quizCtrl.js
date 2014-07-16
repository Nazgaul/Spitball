var mQuiz = angular.module('mQuiz', []);
mQuiz.controller('QuizCtrl',
        ['$scope','$routeParams',
        function ($scope, $routeParams) {
            cd.pubsub.publish('initQuizView');
            cd.pubsub.publish('quiz');
            cd.pubsub.publish('quiz', $routeParams.quizId);//statistics

            //todo proper return;
        }
        ]);
