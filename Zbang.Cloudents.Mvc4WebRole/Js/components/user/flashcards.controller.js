var app;
(function (app) {
    "use strict";
    var Flashcard = (function () {
        function Flashcard(profileData, userService) {
            this.profileData = profileData;
            this.userService = userService;
            this.flashcard = [];
            this.flashcardLoading = false;
            this.flashcardPage = 0;
            this.loadFlashcard();
        }
        Flashcard.prototype.loadFlashcard = function () {
            var _this = this;
            var self = this;
            self.flashcardLoading = true;
            return this.userService.flashcard(self.profileData.id, self.flashcardPage).then(function (response) {
                for (var i = 0; i < response.length; i++) {
                    response[i].publish = true;
                }
                _this.flashcard = _this.flashcard.concat(response);
                if (response.length) {
                    _this.flashcardLoading = false;
                    self.flashcardPage++;
                }
            });
        };
        Flashcard.$inject = ["profileData", "userService"];
        return Flashcard;
    }());
    angular.module("app.user").controller("userFlashcard", Flashcard);
})(app || (app = {}));
//# sourceMappingURL=flashcards.controller.js.map