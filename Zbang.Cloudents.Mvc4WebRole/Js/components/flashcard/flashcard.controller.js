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
    var FlashCard = (function () {
        function FlashCard() {
        }
        return FlashCard;
    }());
    var Card = (function () {
        function Card() {
            this.front = new app.CardSlide();
            this.cover = new app.CardSlide();
        }
        return Card;
    }());
    var Flashcard = (function () {
        function Flashcard(flashcard, flashcardService, $stateParams) {
            this.flashcard = flashcard;
            this.flashcardService = flashcardService;
            this.$stateParams = $stateParams;
            this.step = Steps.Start;
            this.slidepos = 0;
            angular.forEach(flashcard.cards, function (v, k) {
                v.index = k;
            });
            this.fc = flashcard;
        }
        Flashcard.prototype.start = function () {
            if (shuffle) {
                shuffle(this.fc.cards);
            }
            this.slidepos = 0;
            this.slide = this.fc.cards[this.slidepos];
            this.step = Steps.Memo;
        };
        Flashcard.prototype.prev = function () {
            this.slidepos = Math.max(0, --this.slidepos);
            this.step = Steps.Memo;
            this.slide = this.fc.cards[this.slidepos];
        };
        Flashcard.prototype.next = function () {
            this.slidepos = Math.min(this.fc.cards.length, ++this.slidepos);
            if (this.slidepos === this.fc.cards.length) {
                this.step = Steps.End;
                return;
            }
            this.slide = this.fc.cards[this.slidepos];
        };
        Flashcard.prototype.pin = function () {
            this.slide.pin = !this.slide.pin;
            if (this.slide.pin) {
                this.flashcardService.pin(this.$stateParams["id"], this.slide.index);
            }
        };
        Flashcard.$inject = ["flashcard", "flashcardService", "$stateParams"];
        return Flashcard;
    }());
    angular.module("app.flashcard").controller("flashcard", Flashcard);
})(app || (app = {}));
