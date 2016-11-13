module app {

    export interface IFlashcardService {
        create(model, boxId: number): angular.IPromise<number>;
        draft(id: number): angular.IPromise<number>;
        update(id: number, model, boxId: number): angular.IPromise<number>;
        deleteImage(id: number, image: string): angular.IPromise<any>;
        publish(id: number, model, boxId: number): angular.IPromise<any>;
        delete(id: number): angular.IPromise<any>;
    }

    class Flashcard implements IFlashcardService {
        static $inject = ["ajaxService2"];
        constructor(private ajaxService2: IAjaxService2) {
        
        }
        create(model, boxId: number) {
            return this.ajaxService2.post("/flashcard/", { model: model, boxId: boxId });
        }
        draft(id: number) {
            return this.ajaxService2.get("/flashcard/draft/", { id: id });
        }
        update(id: number, model, boxId: number) {
            return this.ajaxService2.put("/flashcard/", { id: id, model: model, boxId: boxId });
        }
        deleteImage(id: number, image: string) {
            return this.ajaxService2.delete("/flashcard/image/", { id: id, image: image });
        }

        publish(id: number, model, boxId: number) {
            return this.ajaxService2.post("/flashcard/publish/", { id: id, model: model, boxId: boxId });
        }
        delete(id: number) {
            return this.ajaxService2.delete("/flashcard/", { id: id });
        }
    }
    angular.module("app.flashcard").service("flashcardService", Flashcard);
}