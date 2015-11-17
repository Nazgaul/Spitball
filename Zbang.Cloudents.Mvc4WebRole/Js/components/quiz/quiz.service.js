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
            return ajaxservice.post('/quiz/creatediscussion', data);
        }

        s.removeDiscussion = function (data) {
            return ajaxservice.post('/quiz/deletediscussion', data);            
        }

        s.getNumberOfSolvers = function (data) {
            return ajaxservice.get('/quiz/numberofsolvers', data);
        }

        s.draft = function(quizId) {
            return ajaxservice.get('/quiz/Draft/', { quizId: quizId });
        }
        s.createQuiz = function(boxId,name) {
            return ajaxservice.post('/quiz/create/', {
                boxId: boxId,
                name: name

            });
        }
        s.updateQuiz = function (id, name) {
            return ajaxservice.post('/quiz/update/', {
                id: id,
                name: name
            });
        }
        s.createQuestion = function(quizId,text) {
            return ajaxservice.post('/quiz/createquestion/', {
                quizId: quizId,
                text: text
            });
        }
        s.updateQuestion = function (questionId, text) {
            return ajaxservice.post('/quiz/updatequestion/', {
                id: questionId,
                text: text
            });
        }
        s.createAnswer = function(questionId, text) {
            return ajaxservice.post('/quiz/createanswer/', {
                questionId: questionId,
                text: text
            });
        }
        s.updateAnswer = function (answerId, text) {
            return ajaxservice.post('/quiz/updateanswer/', {
                Id: answerId,
                text: text
            });
        }
        s.markCorrect = function(answerId) {
            return ajaxservice.post('/quiz/markcorrect/', {
                answerId: answerId
            });
        }
        s.deleteAnswer = function(answerId) {
            return ajaxservice.post('/quiz/deleteanswer/', {
                id: answerId
            });
        }
        s.deleteQuestion = function (questionId) {
            return ajaxservice.post('/quiz/deletequestion/', {
                id: questionId
            });
        }
        s.publish = function(quizId) {
            return ajaxservice.post('/quiz/save/', {
                quizId: quizId
            });
        }

        s.deleteQuiz = function(quizId) {
            return ajaxservice.post('/quiz/delete/', {
                id: quizId
            });
        }
    }
})();