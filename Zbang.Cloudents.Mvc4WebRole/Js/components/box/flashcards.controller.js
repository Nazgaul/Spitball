var app;
(function (app) {
    var Flashcards = (function () {
        function Flashcards($stateParams) {
            this.$stateParams = $stateParams;
            this.params = $stateParams;
        }
        Flashcards.$inject = ["$stateParams"];
        return Flashcards;
    }());
    angular.module('app.box.flashcards').controller('FlashcardsController', Flashcards);
})(app || (app = {}));
