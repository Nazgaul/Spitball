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
            this.flashcardService = flashcardService;
            this.$stateParams = $stateParams;
            this.step = Steps.Start;
            this.slidepos = 0;
            this.disabled = false;
            this.style = true;
            this.pinCount = 0;
            angular.forEach(flashcard.cards, function (v, k) {
                if (flashcard.pins.indexOf(k) !== -1) {
                    v.pin = true;
                }
                v.index = k;
            });
            this.flashcard = flashcard;
            this.pinCount = flashcard.pins.length;
        }
        FlashcardController.prototype.start = function () {
            this.fc = this.flashcard.cards.slice(0);
            this.goToStep2();
        };
        FlashcardController.prototype.goToStep2 = function () {
            if (this.shuffle) {
                shuffle(this.fc);
            }
            this.slidepos = 0;
            this.slide = this.fc[this.slidepos];
            this.step = Steps.Memo;
        };
        FlashcardController.prototype.startPin = function () {
            this.fc = this.flashcard.cards.filter(function (f) { return f.pin; });
            this.goToStep2();
        };
        FlashcardController.prototype.prev = function () {
            this.slidepos = Math.max(0, --this.slidepos);
            this.step = Steps.Memo;
            if (this.style !== null) {
                this.style = true;
            }
            this.slide = this.fc[this.slidepos];
        };
        FlashcardController.prototype.next = function () {
            this.slidepos = Math.min(this.fc.length, ++this.slidepos);
            if (this.slidepos === this.fc.length) {
                this.step = Steps.End;
                return;
            }
            if (this.style !== null) {
                this.style = true;
            }
            this.slide = this.fc[this.slidepos];
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
            if (!this.flashcard.like) {
                this.flashcardService.like(this.$stateParams["id"]).then(function (response) { return _this.flashcard.like = response; }).finally(function () { return _this.disabled = false; });
            }
            else {
                this.flashcardService.likeDelete(this.flashcard.like).then(function () { return _this.flashcard.like = null; }).finally(function () { return _this.disabled = false; });
            }
        };
        FlashcardController.$inject = ["flashcard", "flashcardService", "$stateParams"];
        return FlashcardController;
    }());
    angular.module("app.flashcard").controller("flashcard", FlashcardController);
})(app || (app = {}));
