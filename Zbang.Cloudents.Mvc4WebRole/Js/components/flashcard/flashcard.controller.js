var app;
(function (app) {
    "use strict";
    var Steps;
    (function (Steps) {
        Steps[Steps["Start"] = 0] = "Start";
        Steps[Steps["Memo"] = 1] = "Memo";
        Steps[Steps["End"] = 2] = "End";
    })(Steps || (Steps = {}));
    ;
    function shuffle(a) {
        var j, x, i;
        for (i = a.length; i; i--) {
            j = Math.floor(Math.random() * i);
            x = a[i - 1];
            a[i - 1] = a[j];
            a[j] = x;
        }
    }
    var Flashcard = (function () {
        function Flashcard() {
        }
        return Flashcard;
    }());
    var Card = (function () {
        function Card() {
            this.front = new app.CardSlide();
            this.cover = new app.CardSlide();
        }
        return Card;
    }());
    var FlashcardController = (function () {
        function FlashcardController(flashcard, flashcardService, $stateParams) {
            this.flashcard = flashcard;
            this.flashcardService = flashcardService;
            this.$stateParams = $stateParams;
            this.step = Steps.Start;
            this.slidepos = 0;
            this.disabled = false;
            console.log(flashcard);
            angular.forEach(flashcard.cards, function (v, k) {
                if (flashcard.pins.indexOf(k) !== -1) {
                    v.pin = true;
                }
                v.index = k;
            });
            this.fc = flashcard;
        }
        FlashcardController.prototype.start = function () {
            if (this.shuffle) {
                shuffle(this.fc.cards);
            }
            this.slidepos = 0;
            this.slide = this.fc.cards[this.slidepos];
            this.step = Steps.Memo;
        };
        FlashcardController.prototype.prev = function () {
            this.slidepos = Math.max(0, --this.slidepos);
            this.step = Steps.Memo;
            this.slide = this.fc.cards[this.slidepos];
        };
        FlashcardController.prototype.next = function () {
            this.slidepos = Math.min(this.fc.cards.length, ++this.slidepos);
            if (this.slidepos === this.fc.cards.length) {
                this.step = Steps.End;
                return;
            }
            this.slide = this.fc.cards[this.slidepos];
        };
        FlashcardController.prototype.pin = function () {
            this.slide.pin = !this.slide.pin;
            if (this.slide.pin) {
                this.flashcardService.pin(this.$stateParams["id"], this.slide.index);
            }
            else {
                this.flashcardService.pinDelete(this.$stateParams["id"], this.slide.index);
            }
        };
        FlashcardController.prototype.like = function () {
            var _this = this;
            this.disabled = true;
            if (!this.fc.like) {
                this.flashcardService.like(this.$stateParams["id"]).then(function (response) { return _this.fc.like = response; }).finally(function () { return _this.disabled = false; });
            }
            else {
                this.flashcardService.likeDelete(this.fc.like).then(function () { return _this.fc.like = null; }).finally(function () { return _this.disabled = false; });
            }
        };
        FlashcardController.$inject = ["flashcard", "flashcardService", "$stateParams"];
        return FlashcardController;
    }());
    angular.module("app.flashcard").controller("flashcard", FlashcardController);
})(app || (app = {}));
