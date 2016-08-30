/// <reference path="quizCreate2.controller.ts" />
var app;
(function (app) {
    'use strict';
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
            });
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
        //createAnswer(questionId: string, text: string) {
        //    return this.ajaxService.post('/quiz/createanswer/', {
        //        questionId: questionId,
        //        text: text
        //    });
        //}
        //updateAnswer(answerId: string, text: string) {
        //    return this.ajaxService.post('/quiz/updateanswer/', {
        //        Id: answerId,
        //        text: text
        //    });
        //}
        //markCorrect(answerId: string) {
        //    return this.ajaxService.post('/quiz/markcorrect/', {
        //        answerId: answerId
        //    });
        //}
        //deleteAnswer(answerId: string) {
        //    return this.ajaxService.post('/quiz/deleteanswer/', {
        //        id: answerId
        //    });
        //}
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
        QuizService.$inject = ['ajaxService2'];
        return QuizService;
    }());
    angular.module("app.quiz").service('quizService', QuizService);
})(app || (app = {}));
//(function () {
//    angular.module('app.quiz').service('quizService', service);
//    service.$inject = ['ajaxService'];
//    function service(ajaxservice) {
//        var s = this;
//        s.getQuiz = function (boxId, quizId) {
//            return ajaxservice.get('/quiz/data/', { boxId: boxId, quizId: quizId });
//        }
//        s.saveAnswers = function (data) {
//            return ajaxservice.post('/quiz/saveAnswers', data);
//        }
//        s.getDiscussion = function (data) {
//            return ajaxservice.get('/quiz/discussion', data);
//        }
//        s.createDiscussion = function (data) {
//            return ajaxservice.post('/quiz/creatediscussion', data);
//        }
//        s.removeDiscussion = function (data) {
//            return ajaxservice.post('/quiz/deletediscussion', data);
//        }
//        s.getNumberOfSolvers = function (data) {
//            return ajaxservice.get('/quiz/numberofsolvers', data);
//        }
//        s.draft = function (quizId) {
//            return ajaxservice.get('/quiz/draft/', { quizId: quizId });
//        }
//        s.createQuiz = function (boxId, name) {
//            return ajaxservice.post('/quiz/create/', {
//                boxId: boxId,
//                name: name
//            });
//        }
//        s.updateQuiz = function (id, name) {
//            return ajaxservice.post('/quiz/update/', {
//                id: id,
//                name: name
//            });
//        }
//        s.createQuestion = function (quizId, text) {
//            return ajaxservice.post('/quiz/createquestion/', {
//                quizId: quizId,
//                text: text
//            });
//        }
//        s.updateQuestion = function (questionId, text) {
//            return ajaxservice.post('/quiz/updatequestion/', {
//                id: questionId,
//                text: text
//            });
//        }
//        s.createAnswer = function (questionId, text) {
//            return ajaxservice.post('/quiz/createanswer/', {
//                questionId: questionId,
//                text: text
//            });
//        }
//        s.updateAnswer = function (answerId, text) {
//            return ajaxservice.post('/quiz/updateanswer/', {
//                Id: answerId,
//                text: text
//            });
//        }
//        s.markCorrect = function (answerId) {
//            return ajaxservice.post('/quiz/markcorrect/', {
//                answerId: answerId
//            });
//        }
//        s.deleteAnswer = function (answerId) {
//            return ajaxservice.post('/quiz/deleteanswer/', {
//                id: answerId
//            });
//        }
//        s.deleteQuestion = function (questionId) {
//            return ajaxservice.post('/quiz/deletequestion/', {
//                id: questionId
//            });
//        }
//        s.publish = function (quizId) {
//            return ajaxservice.post('/quiz/save/', {
//                quizId: quizId
//            });
//        }
//        s.deleteQuiz = function (quizId) {
//            return ajaxservice.post('/quiz/delete/', {
//                id: quizId
//            });
//        }
//    }
//})(); 
