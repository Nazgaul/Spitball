"use strict";
var app;
(function (app) {
    var Flashcard = (function () {
        function Flashcard(ajaxService2) {
            this.ajaxService2 = ajaxService2;
        }
        Flashcard.prototype.create = function (model, boxId) {
            return this.ajaxService2.post("/flashcard/", { model: model, boxId: boxId }, "boxData").then(function (r) { return r; });
        };
        Flashcard.prototype.get = function (id, boxId) {
            return this.ajaxService2.get("/flashcard/data/", { id: id, boxId: boxId });
        };
        Flashcard.prototype.draft = function (id) {
            return this.ajaxService2.get("/flashcard/draft/", { id: id }).then(function (r) { return r; });
        };
        Flashcard.prototype.update = function (id, model, boxId) {
            return this.ajaxService2.put("/flashcard/", { id: id, model: model, boxId: boxId }).then(function (r) { return r; });
        };
        Flashcard.prototype.deleteImage = function (id, image) {
            return this.ajaxService2.delete("/flashcard/image/", { id: id, image: image });
        };
        Flashcard.prototype.publish = function (id, model, boxId) {
            return this.ajaxService2.post("/flashcard/publish/", { id: id, model: model, boxId: boxId });
        };
        Flashcard.prototype.delete = function (id) {
            return this.ajaxService2.delete("/flashcard/", { id: id });
        };
        Flashcard.prototype.pin = function (id, index) {
            return this.ajaxService2.post("/flashcard/pin/", { id: id, index: index });
        };
        Flashcard.prototype.pinDelete = function (id, index) {
            return this.ajaxService2.delete("/flashcard/pin/", { id: id, index: index });
        };
        Flashcard.prototype.like = function (id) {
            return this.ajaxService2.post("/flashcard/like/", { id: id }).then(function (r) { return r; });
        };
        Flashcard.prototype.likeDelete = function (id) {
            return this.ajaxService2.delete("/flashcard/unlike/", { id: id });
        };
        Flashcard.prototype.solve = function (id) {
            return this.ajaxService2.post("/flashcard/solve/", { id: id });
        };
        return Flashcard;
    }());
    Flashcard.$inject = ["ajaxService2"];
    angular.module("app.flashcard").service("flashcardService", Flashcard);
})(app || (app = {}));
//# sourceMappingURL=flashcard.service.js.map