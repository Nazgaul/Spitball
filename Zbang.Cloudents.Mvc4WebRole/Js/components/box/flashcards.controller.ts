module app {

    class Flashcards {
        static $inject = ["$stateParams"];
        params;
        constructor(private $stateParams: angular.ui.IStateParamsService) {
            this.params = $stateParams;
        }
    }
    angular.module('app.box.flashcards').controller('FlashcardsController', Flashcards);
}