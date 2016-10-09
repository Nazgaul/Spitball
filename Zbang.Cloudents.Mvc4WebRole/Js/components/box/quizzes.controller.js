(function () {
'use strict';
    angular.module('app.box.quizzes').controller('QuizzesController', quizzes);
    quizzes.$inject = ['boxService', '$stateParams', '$mdDialog', 'quizService', 'boxData', '$mdMedia', '$state', 'user', '$rootScope', 'resManager'];

    function quizzes(boxService, $stateParams, $mdDialog, quizService, boxData, $mdMedia, $state, user, $rootScope, resManager) {
        var q = this;
        q.params = $stateParams;
        q.deleteQuiz = deleteQuiz;
        q.createQuiz = createQuiz;
        q.quizzes = [];
        boxService.getQuizzes($stateParams.boxId).then(function (response) {

            for (var i = 0; i < response.length; i++) {
                var quiz = response[i];

                if (!quiz.publish) {
                    if ($mdMedia('xs') || $mdMedia('sm')) {
                        continue;
                    }
                    var params = angular.copy($stateParams);
                    params.quizid = quiz.id;
                    params.name = boxData.name;
                    var url = $state.href('quizCreate', params);
                    quiz.url = url;
                }
                q.quizzes.push(quiz);
            }
        });

        function createQuiz(event) {
            if (!user.id) {
                event.preventDefault();
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
        }

        function deleteQuiz(ev, quiz) {
            var confirm = $mdDialog.confirm()
                .title(resManager.get('deleteQuiz'))
                .targetEvent(ev)
                 .ok(resManager.get('dialogOk'))
                 .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = q.quizzes.lastIndexOf(quiz);
                q.quizzes.splice(index, 1);
                quizService.deleteQuiz(quiz.id);
            });
        }


    }
})();