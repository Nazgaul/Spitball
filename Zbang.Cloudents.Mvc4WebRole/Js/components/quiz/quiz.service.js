(function () {
    angular.module('app.quiz').service('quizService', service);
    service.$inject = ['ajaxService'];

    function service(ajaxservice) {
        var s = this;

        s.getQuiz = function (boxId, quizId) {
            return ajaxservice.get('/quiz/data/', { boxId: boxId, quizId: quizId });
        }

        s.saveAnswers = function (data) {
            return ajaxservice.post('/quiz/saveAnswers', data);
        }

        s.getDiscussion = function (data) {
            return ajaxservice.get('/quiz/discussion', data);
        }

        s.createDiscussion = function (data) {
            return ajaxservice.post('/quiz/discussion', data);
        }
    }
})();