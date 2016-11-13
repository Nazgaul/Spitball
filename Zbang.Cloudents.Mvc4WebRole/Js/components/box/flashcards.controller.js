var app;
(function (app) {
    "use strict";
    var Flashcards = (function () {
        function Flashcards($stateParams, flashcards, $state) {
            this.$stateParams = $stateParams;
            this.$state = $state;
            this.params = $stateParams;
            angular.forEach(flashcards, function (val) {
                if (val.publish) {
                    val.url = $state.href("flashcard", {
                        universityName: $stateParams["universityType"],
                        boxId: $stateParams["boxId"],
                        boxName: $stateParams["boxName"],
                        id: val.id,
                        name: val.name
                    });
                }
                else {
                    val.url = $state
                        .href("flashcardCreate", angular.extend({}, $stateParams, { id: val.id }));
                }
            });
            this.flashcards = flashcards;
        }
        Flashcards.$inject = ["$stateParams", "flashcards", "$state"];
        return Flashcards;
    }());
    angular.module('app.box.flashcards').controller('FlashcardsController', Flashcards);
})(app || (app = {}));
