﻿module app {
    "use strict";
    class Flashcards {
        static $inject = ["$stateParams", "flashcards", "$state"];
        params;
        flashcards;
        constructor(
            private $stateParams: angular.ui.IStateParamsService,
            flashcards: Array<any>,
            private $state: angular.ui.IStateService) {
            this.params = $stateParams;
            angular.forEach(flashcards,
                val => {
                    if (val.publish) {
                        val.url = $state.href("flashcard",
                        {
                            universityName: $stateParams["universityType"],
                            boxId: $stateParams["boxId"],
                            boxName: $stateParams["boxName"],
                            id: val.id,
                            name: val.name
                        });
                    } else {
                        val.url = $state
                            .href("flashcardCreate", angular.extend({}, $stateParams, { id: val.id }));
                    }
                });
            this.flashcards = flashcards;
        }
    }
    angular.module('app.box.flashcards').controller('FlashcardsController', Flashcards);
}