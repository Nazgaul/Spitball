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
        constructor(flashcard: FlashCard) {
            console.log(flashcard);
            this.flashcard = flashcard;
        }

        start() {
            if (shuffle) {
                shuffle(this.flashcard.cards);
                console.log(this.flashcard.cards);
            }
            this.step = Steps.Memo;
        }
    }

    angular.module("app.flashcard").controller("flashcard", Flashcard);
}