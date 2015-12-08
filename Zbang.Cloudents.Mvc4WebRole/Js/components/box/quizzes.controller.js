(function () {
    angular.module('app.box.quizzes').controller('QuizzesController', quizzes);
    quizzes.$inject = ['boxService', '$stateParams', '$location', '$mdDialog', 'quizService', 'boxData'];

    function quizzes(boxService, $stateParams, $location, $mdDialog, quizService, boxData) {
        var q = this;
        q.params = $stateParams;
        q.deleteQuiz = deleteQuiz;
        boxService.getQuizzes($stateParams.boxId).then(function (response) {
            for (var i = 0; i < response.length; i++) {
                var quiz = response[i];
                if (!quiz.publish) {
                    var path = $location.path() + 'quizcreate/?quizid=' + quiz.id + '&name=' + boxData.name;
                    quiz.url = path;
                }
            }
            q.quizzes = response;
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