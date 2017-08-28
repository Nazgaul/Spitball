"use strict";
var app;
(function (app) {
    "use strict";
    var Flashcards = (function () {
        function Flashcards($stateParams, flashcards, $state, $mdDialog, resManager, flashcardService, user, shareService, $scope) {
            var _this = this;
            this.$stateParams = $stateParams;
            this.$state = $state;
            this.$mdDialog = $mdDialog;
            this.resManager = resManager;
            this.flashcardService = flashcardService;
            this.user = user;
            this.shareService = shareService;
            this.$scope = $scope;
            this.params = $stateParams;
            angular.forEach(flashcards, function (val) {
                if (val.publish) {
                    val.url = $state.href("flashcard", {
                        universityType: $stateParams["universityType"],
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
            this.$scope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState) {
                //need indication that he actually created one
                if (fromState.name === "flashcardCreate" && toParams["newId"]) {
                    var flashcard = _this.flashcards.find(function (f) { return f.ownerId === user.id && toParams["newId"] === f.id; });
                    if (flashcard && flashcard.publish) {
                        _this.shareService.shareDialog("f", flashcard.id);
                    }
                }
            });
        }
        Flashcards.prototype.deleteFlashcard = function (ev, flashcard) {
            var _this = this;
            var confirm = this.$mdDialog.confirm()
                .title(this.resManager.get("deleteObj").format(this.resManager.get("flashcard")))
                .targetEvent(ev)
                .ok(this.resManager.get("dialogOk"))
                .cancel(this.resManager.get("dialogCancel"));
            this.$mdDialog.show(confirm).then(function () {
                var index = _this.flashcards.indexOf(flashcard);
                _this.flashcards.splice(index, 1);
                _this.flashcardService.delete(flashcard.id);
            });
        };
        Flashcards.prototype.canEdit = function (flashcard) {
            return this.user.id === flashcard.ownerId && flashcard.publish;
        };
        Flashcards.$inject = ["$stateParams", "flashcards", "$state",
            "$mdDialog", "resManager", "flashcardService", "user", "shareService", "$scope"];
        return Flashcards;
    }());
    angular.module('app.box.flashcards').controller('FlashcardsController', Flashcards);
})(app || (app = {}));
//# sourceMappingURL=flashcards.controller.js.map