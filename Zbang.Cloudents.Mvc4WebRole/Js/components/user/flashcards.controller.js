var app;
(function (app) {
    "use strict";
    var Flashcard = (function () {
        function Flashcard(profileData, userService, $state) {
            this.profileData = profileData;
            this.userService = userService;
            this.$state = $state;
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
                    var elem = response[i];
                    elem.publish = true;
                    elem.url = _this.$state.href("flashcard", {
                        universityType: elem.universityName,
                        boxId: elem.boxId,
                        boxName: elem.boxName,
                        id: elem.id,
                        name: elem.name
                    });
                }
                _this.flashcard = _this.flashcard.concat(response);
                if (response.length) {
                    _this.flashcardLoading = false;
                    self.flashcardPage++;
                }
            });
        };
        Flashcard.$inject = ["profileData", "userService", "$state"];
        return Flashcard;
    }());
    angular.module("app.user").controller("userFlashcard", Flashcard);
})(app || (app = {}));
