var app;
(function (app) {
    "use strict";
    var Flashcard = (function () {
        function Flashcard() {
        }
        return Flashcard;
    }());
    angular.module("app.flashcard").controller("flashcard", Flashcard);
})(app || (app = {}));
