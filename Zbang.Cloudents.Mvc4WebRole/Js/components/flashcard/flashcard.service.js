var app;
(function (app) {
    var Flashcard = (function () {
        function Flashcard(ajaxService2) {
            this.ajaxService2 = ajaxService2;
        }
        Flashcard.prototype.create = function (model, boxId) {
            return this.ajaxService2.post("/flashcard/", { model: model, boxId: boxId });
        };
        Flashcard.prototype.draft = function (id) {
            return this.ajaxService2.get("/flashcard/draft/", { id: id });
        };
        Flashcard.$inject = ["ajaxService2"];
        return Flashcard;
    }());
    angular.module("app.flashcard").service("flashcardService", Flashcard);
})(app || (app = {}));
