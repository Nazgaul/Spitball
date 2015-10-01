(function () {
    angular.module('app.quiz').service('quizService', service);
    service.$inject = ['ajaxService'];

    function service(ajaxservice) {
        var s = this;

        s.getQuiz = function (boxId, quizId) {
            return ajaxservice.get('/quiz/data/', { boxId: boxId, quizId: quizId});
        }

    }
})();