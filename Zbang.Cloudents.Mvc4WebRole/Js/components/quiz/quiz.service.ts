/// <reference path="quizCreate2.controller.ts" />
module app {
    'use strict';
    export interface IQuizService {
        getQuiz(boxId: number, quizId: number): angular.IPromise<any>;
        saveAnswers(data): angular.IPromise<any>;
        getDiscussion(data): angular.IPromise<any>;
        createDiscussion(data): angular.IPromise<any>;
        removeDiscussion(data): angular.IPromise<any>;
        getNumberOfSolvers(data): angular.IPromise<any>;
        draft(quizId: number): angular.IPromise<any>;
        createQuiz(boxId: number, name: string): angular.IPromise<any>;
        updateQuiz(id: number, name: string): angular.IPromise<any>;
        createQuestion(quizId: number, quiz: Question): angular.IPromise<any>;
        updateQuestion(questionId: string, text: string): angular.IPromise<any>;
        //createAnswer(questionId: string, text: string): angular.IPromise<any>;
        //updateAnswer(answerId: string, text: string): angular.IPromise<any>;
        //markCorrect(answerId): angular.IPromise<any>;
        //deleteAnswer(answerId: string): angular.IPromise<any>;
        deleteQuestion(questionId): angular.IPromise<any>;
        publish(quizId: number): angular.IPromise<any>;
        deleteQuiz(quizId: number): angular.IPromise<any>;
    }
    class QuizService implements IQuizService {
        static $inject = ['ajaxService2'];

        constructor(private ajaxService: IAjaxService2) {

        }
        getQuiz(boxId: number, quizId: number) {
            return this.ajaxService.get("/quiz/data/", { boxId: boxId, quizId: quizId });
        }
        saveAnswers(data: any) {
            return this.ajaxService.post('/quiz/saveAnswers', data);
        }
        getDiscussion(data) {
            return this.ajaxService.get('/quiz/discussion', data);
        }
        createDiscussion(data) {
            return this.ajaxService.post('/quiz/creatediscussion', data);
        }
        removeDiscussion(data) {
            return this.ajaxService.post('/quiz/deletediscussion', data);
        }
        getNumberOfSolvers(data) {
            return this.ajaxService.get('/quiz/numberofsolvers', data);
        }
        draft(quizId: number) {
            return this.ajaxService.get('/quiz/draft/', { quizId: quizId });
        }
        createQuiz(boxId: number, name: string) {
            return this.ajaxService.post('/quiz/create/', {
                boxId: boxId,
                name: name

            });
        }
        updateQuiz(id: number, name: string) {
            return this.ajaxService.post('/quiz/update/', {
                id: id,
                name: name
            });
        }
        createQuestion(quizId: number, question: Question) {
            return this.ajaxService.post('/quiz/createquestion/', {
                quizId: quizId,
                model: question
            });
        }
        updateQuestion(questionId: string, text: string) {
            return this.ajaxService.post('/quiz/updatequestion/', {
                id: questionId,
                text: text
            });
        }
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
        deleteQuestion(questionId) {
            return this.ajaxService.post('/quiz/deletequestion/', {
                id: questionId
            });
        }
        publish(quizId: number) {
            return this.ajaxService.post('/quiz/save/', {
                quizId: quizId
            });
        }

        deleteQuiz(quizId: number) {
            return this.ajaxService.post('/quiz/delete/', {
                id: quizId
            });
        }
    }

    angular.module("app.quiz").service('quizService', QuizService);

}

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