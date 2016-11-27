var app;
(function (app) {
    "use strict";
    var QuizService = (function () {
        function QuizService(ajaxService) {
            this.ajaxService = ajaxService;
        }
        QuizService.prototype.getQuiz = function (boxId, quizId) {
            return this.ajaxService.get("/quiz/data/", { boxId: boxId, quizId: quizId });
        };
        QuizService.prototype.saveAnswers = function (data) {
            return this.ajaxService.post('/quiz/saveAnswers', data);
        };
        QuizService.prototype.getDiscussion = function (data) {
            return this.ajaxService.get('/quiz/discussion', data);
        };
        QuizService.prototype.createDiscussion = function (data) {
            return this.ajaxService.post('/quiz/creatediscussion', data);
        };
        QuizService.prototype.removeDiscussion = function (data) {
            return this.ajaxService.post('/quiz/deletediscussion', data);
        };
        QuizService.prototype.getNumberOfSolvers = function (data) {
            return this.ajaxService.get('/quiz/numberofsolvers', data);
        };
        QuizService.prototype.draft = function (quizId) {
            return this.ajaxService.get('/quiz/draft/', { quizId: quizId });
        };
        QuizService.prototype.createQuiz = function (boxId, name) {
            return this.ajaxService.post('/quiz/create/', {
                boxId: boxId,
                name: name
            }, "boxData");
        };
        QuizService.prototype.updateQuiz = function (id, name) {
            return this.ajaxService.post('/quiz/update/', {
                id: id,
                name: name
            });
        };
        QuizService.prototype.createQuestion = function (quizId, question) {
            return this.ajaxService.post('/quiz/createquestion/', {
                quizId: quizId,
                model: question
            });
        };
        QuizService.prototype.updateQuestion = function (questionId, text) {
            return this.ajaxService.post('/quiz/updatequestion/', {
                id: questionId,
                text: text
            });
        };
        QuizService.prototype.deleteQuestion = function (questionId) {
            return this.ajaxService.post('/quiz/deletequestion/', {
                id: questionId
            });
        };
        QuizService.prototype.publish = function (quizId) {
            return this.ajaxService.post('/quiz/save/', {
                quizId: quizId
            });
        };
        QuizService.prototype.deleteQuiz = function (quizId) {
            return this.ajaxService.post('/quiz/delete/', {
                id: quizId
            });
        };
        QuizService.$inject = ["ajaxService2"];
        return QuizService;
    }());
    angular.module("app.quiz").service('quizService', QuizService);
})(app || (app = {}));
