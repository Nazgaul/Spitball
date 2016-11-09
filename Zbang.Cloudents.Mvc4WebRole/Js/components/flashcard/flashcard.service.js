var app;
(function (app) {
    var Flashcard = (function () {
        function Flashcard(ajaxService2) {
            this.ajaxService2 = ajaxService2;
        }
        Flashcard.prototype.create = function (model) {
            return this.ajaxService2.post("/flashcard/create/", model);
        };
        Flashcard.$inject = ["ajaxService2"];
        return Flashcard;
    }());
    angular.module("app.flashcard").service("flashcardService", Flashcard);
})(app || (app = {}));
