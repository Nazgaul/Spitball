(function () {
    angular.module('app.box.quizzes').controller('QuizzesController', quizzes);
    quizzes.$inject = ['boxService', '$stateParams'];

    function quizzes(boxService, $stateParams) {
        var q = this;        
        boxService.getQuizzes($stateParams.boxId).then(function (response) {
            q.quizzes = response;
        });
    }
})();