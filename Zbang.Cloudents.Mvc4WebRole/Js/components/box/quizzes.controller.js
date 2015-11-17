(function () {
    angular.module('app.box.quizzes').controller('QuizzesController', quizzes);
    quizzes.$inject = ['boxService', '$stateParams', '$location'];

    function quizzes(boxService, $stateParams, $location) {
        var q = this;
        boxService.getQuizzes($stateParams.boxId).then(function (response) {
            for (var i = 0; i < response.length; i++) {
                var quiz = response[i];
                if (!quiz.publish) {
                    var path = $location.path() + 'quizcreate/?quizid=' + quiz.id;
                    quiz.url = path;
                }
            }
            q.quizzes = response;
        });
    }
})();