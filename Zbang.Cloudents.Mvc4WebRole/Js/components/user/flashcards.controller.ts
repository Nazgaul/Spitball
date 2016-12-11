module app {
    "use strict";
    class Flashcard {
        flashcard = [];
        flashcardLoading = false;
        flashcardPage = 0;
        static $inject = ["profileData", "userService"];
        constructor(
            private profileData: IUserData,
            private userService: IUserService
        ) {
            this.loadFlashcard();
        }

        loadFlashcard() {
            var self = this;
            self.flashcardLoading = true;
            return this.userService.flashcard(self.profileData.id, self.flashcardPage).then(response => {
                for (let i = 0; i < response.length; i++) {
                    response[i].publish = true;
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