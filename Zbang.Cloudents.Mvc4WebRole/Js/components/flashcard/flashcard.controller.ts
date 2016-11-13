module app {
    "use strict";

    enum Steps {
        Start,
        Memo,
        End
    };
    function shuffle(a) {
        var j, x, i;
        for (i = a.length; i; i--) {
            j = Math.floor(Math.random() * i);
            x = a[i - 1];
            a[i - 1] = a[j];
            a[j] = x;
        }
    }
    class Flashcard {


        static $inject = ["flashcard"];
        flashcard: FlashCard;
        shuffle: boolean;
        step = Steps.Start;
        slidepos = 0;
        slide: Card;
        constructor(flashcard: FlashCard) {
            console.log(flashcard);
            this.flashcard = flashcard;
        }

        start() {
            if (shuffle) {
                shuffle(this.flashcard.cards);
            }
            this.slidepos = 0;
            this.slide = this.flashcard.cards[this.slidepos];
            this.step = Steps.Memo;
        }
        prev() {
            this.slidepos = Math.max(0, --this.slidepos);
            this.step = Steps.Memo;
            this.slide = this.flashcard.cards[this.slidepos];
        }
        next() {
            this.slidepos = Math.min(this.flashcard.cards.length, ++this.slidepos);
            if (this.slidepos === this.flashcard.cards.length) {
                this.step = Steps.End;
                return;
            }
            this.slide = this.flashcard.cards[this.slidepos];
        }
    }

    angular.module("app.flashcard").controller("flashcard", Flashcard);
}