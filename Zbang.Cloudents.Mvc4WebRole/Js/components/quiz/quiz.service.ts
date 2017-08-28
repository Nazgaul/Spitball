/// <reference path="quizCreate2.controller.ts" />
module app {
    "use strict";
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
        deleteQuestion(questionId): angular.IPromise<any>;
        publish(quizId: number): angular.IPromise<any>;
        deleteQuiz(quizId: number): angular.IPromise<any>;
        like(id: number): angular.IPromise<number>;
        likeDelete(id: Guid): angular.IPromise<any>;
    }
    class QuizService implements IQuizService {
        static $inject = ["ajaxService2"];

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

            },"boxData");
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
        like(id: number): angular.IPromise<number> {
            return this.ajaxService.post("/quiz/like/", { id: id }).then(p => {
                return p as number;
            });
            //return this.ajaxService.post("/quiz/like/", { id: id });
        }
        likeDelete(id: Guid) {
            return this.ajaxService.delete("/quiz/like/", { id: id });
        }

    }

    angular.module("app.quiz").service('quizService', QuizService);

}
