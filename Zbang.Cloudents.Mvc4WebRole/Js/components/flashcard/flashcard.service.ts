module app {

    export interface IFlashcardService {
        create(model, boxId: number): angular.IPromise<number>;
        draft(id: number): angular.IPromise<number>;
        update(id: number, model, boxId: number): angular.IPromise<number>;
        deleteImage(id: number, image: string): angular.IPromise<any>;
        publish(id: number, model, boxId: number): angular.IPromise<any>;
        delete(id: number): angular.IPromise<any>;
        get(id: number, boxId: number): angular.IPromise<any>;
        pin(id: number, index: number): angular.IPromise<any>;
        pinDelete(id: number, index: number): angular.IPromise<any>;
        like(id: number): angular.IPromise<number>;
        likeDelete(id: Guid): angular.IPromise<any>;
        solve(id: number): angular.IPromise<any>;
    }

    class Flashcard implements IFlashcardService {
        static $inject = ["ajaxService2"];
        constructor(private ajaxService2: IAjaxService2) {
        
        }
        create(model, boxId: number): angular.IPromise<number> {
            return this.ajaxService2.post("/flashcard/", { model: model, boxId: boxId }, "boxData").then(r=> r as number);
        }
        get(id: number, boxId: number) {
            return this.ajaxService2.get("/flashcard/data/", { id: id, boxId:boxId});
        }
        draft(id: number): angular.IPromise<number> {
            return this.ajaxService2.get("/flashcard/draft/", { id: id }).then(r => r as number);
        }
        update(id: number, model, boxId: number): angular.IPromise<number> {
            return this.ajaxService2.put("/flashcard/", { id: id, model: model, boxId: boxId }).then(r => r as number);
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
        pin(id: number,index:number) {
            return this.ajaxService2.post("/flashcard/pin/", { id: id ,index:index});
        }
        pinDelete(id: number, index: number) {
            return this.ajaxService2.delete("/flashcard/pin/", { id: id, index: index });
        }
        like(id: number): angular.IPromise<number> {
            return this.ajaxService2.post("/flashcard/like/", { id: id }).then(r => r as number);
        }
        likeDelete(id: Guid) {
            return this.ajaxService2.delete("/flashcard/unlike/", { id: id });
        }
        solve(id: number) {
            return this.ajaxService2.post("/flashcard/solve/", { id: id });
        }
    }
    angular.module("app.flashcard").service("flashcardService", Flashcard);
}