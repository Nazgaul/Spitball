var mQuiz = angular.module('mQuiz', []);
mQuiz.controller('QuizCtrl',
        ['$scope',
        function ($scope) {
            cd.pubsub.publish('initQuizView');
            cd.pubsub.publish('quiz');

            //todo proper return;
        }
        ]);
