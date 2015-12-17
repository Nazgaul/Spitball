(function () {
    angular.module('app.box.quizzes').controller('QuizzesController', quizzes);
    quizzes.$inject = ['boxService', '$stateParams', '$mdDialog', 'quizService', 'boxData', '$mdMedia', '$state'];

    function quizzes(boxService, $stateParams,  $mdDialog, quizService, boxData, $mdMedia, $state) {
        var q = this;
        q.params = $stateParams;
        q.deleteQuiz = deleteQuiz;
        q.quizzes = [];
        boxService.getQuizzes($stateParams.boxId).then(function (response) {

            for (var i = 0; i < response.length; i++) {
                var quiz = response[i];

                if (!quiz.publish) {
                    if ($mdMedia('xs')) {
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



        function deleteQuiz(ev, quiz) {
            var confirm = $mdDialog.confirm()
                .title('Would you like to delete this quiz?')
                //.textContent('All of the banks have agreed to forgive you your debts.')
                .targetEvent(ev)
                .ok('Ok')
                .cancel('Cancel');

            $mdDialog.show(confirm).then(function () {
                var index = q.quizzes.lastIndexOf(quiz);
                q.quizzes.splice(index, 1);
                quizService.deleteQuiz(quiz.id);
            });
        }
    }
})();