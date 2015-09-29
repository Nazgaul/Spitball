(function () {
    angular.module('app.box.quizzes').controller('QuizzesController', quizzes);
    quizzes.$inject = ['boxService', '$stateParams'];

    function quizzes(boxService, $stateParams) {


        var m = this;
        var boxData;
        boxService.getQuizzes($stateParams.boxId).then(function (response) {
            m.quizzes = response;
        });
    }
})();