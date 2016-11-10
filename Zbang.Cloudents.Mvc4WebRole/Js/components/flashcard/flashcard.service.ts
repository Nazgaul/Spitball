﻿module app {

    export interface IFlashcardService {
        create(model, boxId: number): angular.IPromise<number>;
        draft(id: number): angular.IPromise<number>;
        update(id: number, model, boxId: number): angular.IPromise<number>;
    }

    class Flashcard {
        static $inject = ["ajaxService2"];

        constructor(private ajaxService2: IAjaxService2) {

        }
        create(model, boxId: number) {
            return this.ajaxService2.post("/flashcard/", { model: model, boxId: boxId });
        }
        draft(id: number) {
            return this.ajaxService2.get("/flashcard/draft/",{ id: id });
        }
        update(id:number, model, boxId: number) {
            return this.ajaxService2.put("/flashcard/", { id: id, model: model, boxId: boxId });
        }
    }
    angular.module("app.flashcard").service("flashcardService", Flashcard);
}