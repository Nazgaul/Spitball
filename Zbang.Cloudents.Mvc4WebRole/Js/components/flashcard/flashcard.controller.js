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
        function Flashcard(flashcard) {
            this.step = Steps.Start;
            this.slidepos = 0;
            console.log(flashcard);
            this.flashcard = flashcard;
        }
        Flashcard.prototype.start = function () {
            if (shuffle) {
                shuffle(this.flashcard.cards);
            }
            this.slidepos = 0;
            this.slide = this.flashcard.cards[this.slidepos];
            this.step = Steps.Memo;
        };
        Flashcard.prototype.prev = function () {
            this.slidepos = Math.max(0, --this.slidepos);
            this.slide = this.flashcard.cards[this.slidepos];
        };
        Flashcard.prototype.next = function () {
            this.slidepos = Math.min(this.flashcard.cards.length, ++this.slidepos);
            this.slide = this.flashcard.cards[this.slidepos];
        };
        Flashcard.$inject = ["flashcard"];
        return Flashcard;
    }());
    angular.module("app.flashcard").controller("flashcard", Flashcard);
})(app || (app = {}));
