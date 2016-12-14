module app {
    "use strict";
    class Flashcard {
        flashcard = [];
        flashcardLoading = false;
        flashcardPage = 0;
        static $inject = ["profileData", "userService","$state"];
        constructor(
            private profileData: IUserData,
            private userService: IUserService,
            private $state: angular.ui.IStateService
        ) {
            this.loadFlashcard();
        }

        loadFlashcard() {
            var self = this;
            self.flashcardLoading = true;
            return this.userService.flashcard(self.profileData.id, self.flashcardPage).then(response => {
                for (let i = 0; i < response.length; i++) {
                    const elem = response[i];
                    elem.publish = true;
                    elem.url = this.$state.href("flashcard",
                        {
                            universityType: elem.universityName,
                            boxId: elem.boxId,
                            boxName: elem.boxName,
                            id: elem.id,
                            name: elem.name
                        });
                }
                this.flashcard = this.flashcard.concat(response);
                if (response.length) {
                    this.flashcardLoading = false;
                    self.flashcardPage++;
                }

            });
        }
    }

    angular.module("app.user").controller("userFlashcard", Flashcard);
}