module app {
    "use strict";
    class Flashcards {
        static $inject = ["$stateParams", "flashcards", "$state", "$mdDialog", "resManager", "flashcardService", "user"];
        params : angular.ui.IStateParamsService;
        flashcards: Array<any>;
        constructor(
            private $stateParams: angular.ui.IStateParamsService,
            flashcards: Array<any>,
            private $state: angular.ui.IStateService,
            private $mdDialog: angular.material.IDialogService,
            private resManager: IResManager,
            private flashcardService: IFlashcardService,
            private user: IUserData) {
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

        deleteFlashcard(ev: MouseEvent, flashcard) {
            const confirm = this.$mdDialog.confirm()
                .title(this.resManager.get("deleteObj").format(this.resManager.get("flashcard")))
                .targetEvent(ev)
                .ok(this.resManager.get("dialogOk"))
                .cancel(this.resManager.get("dialogCancel"));

            this.$mdDialog.show(confirm).then(() => {
                var index = this.flashcards.indexOf(flashcard);
                this.flashcards.splice(index, 1);
                this.flashcardService.delete(flashcard.id);
            });
        }
        canEdit(flashcard) {
            return this.user.id === flashcard.ownerId && flashcard.publish;
        }
    }
    angular.module('app.box.flashcards').controller('FlashcardsController', Flashcards);
}