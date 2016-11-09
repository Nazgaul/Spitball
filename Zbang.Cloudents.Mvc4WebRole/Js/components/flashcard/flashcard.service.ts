module app {

    export interface IFlashcardService {
        create(model): angular.IPromise<any>;
    }

    class Flashcard {
        static $inject = ["ajaxService2"];

        constructor(private ajaxService2: IAjaxService2) {

        }
        create(model) {
            return this.ajaxService2.post("/flashcard/create/", model);
        }
    }
    angular.module("app.flashcard").service("flashcardService", Flashcard);
}