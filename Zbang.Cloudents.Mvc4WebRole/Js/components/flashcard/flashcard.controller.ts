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
        name: string;
        cards: Array<Card>;
        pins: Array<number>;
        like: Guid;
        //deserialize(input: FlashCard) {
        //    this.cards = [];
        //    this.id = input.id;
        //    this.name = input.name;
        //    for (let i = 0; i < input.cards.length; i++) {
        //        this.cards.push(new Card().deserialize(input.cards[i]));
        //    }
        //    if (!this.cards.length) {
        //        this.cards.push(new Card(), new Card(), new Card(), new Card(), new Card());
        //    }
        //    return this;
        //}



    }
    class Card {
        front = new CardSlide();
        cover = new CardSlide();
        index: number;
        pin: boolean;
        //deserialize(input: Card) {
        //    this.front = new CardSlide().deserialize(input.front);
        //    this.cover = new CardSlide().deserialize(input.cover);
        //    return this;
        //}
    }
    class FlashcardController {
        static $inject = ["flashcard", "flashcardService", "$stateParams"];
        fc: Flashcard;
        shuffle: boolean;
        step = Steps.Start;
        slidepos = 0;
        slide: Card;
        disabled = false;
        constructor(private flashcard: Flashcard,
            private flashcardService: IFlashcardService,
            private $stateParams: angular.ui.IStateParamsService) {
            angular.forEach(flashcard.cards,
                (v, k) => {
                    if (flashcard.pins.indexOf(k) !== -1) {
                        v.pin = true;
                    }
                    v.index = k;

                });
            this.fc = flashcard;
        }

        start() {
            if (this.shuffle) {
                shuffle(this.fc.cards);
            }
            this.slidepos = 0;
            this.slide = this.fc.cards[this.slidepos];
            this.step = Steps.Memo;
        }
        prev() {
            this.slidepos = Math.max(0, --this.slidepos);
            this.step = Steps.Memo;
            this.slide = this.fc.cards[this.slidepos];
        }
        next() {
            this.slidepos = Math.min(this.fc.cards.length, ++this.slidepos);
            if (this.slidepos === this.fc.cards.length) {
                this.step = Steps.End;
                return;
            }
            this.slide = this.fc.cards[this.slidepos];
        }
        pin() {
            //const index = this.flashcard.cards.indexOf(this.slide);
            //console.log(this.slide, index);
            this.slide.pin = !this.slide.pin;
            if (this.slide.pin) {
                this.flashcardService.pin(this.$stateParams["id"], this.slide.index);
            } else {
                this.flashcardService.pinDelete(this.$stateParams["id"], this.slide.index);
            }
        }
        like() {
            this.disabled = true;
            if (!this.fc.like) {
                this.flashcardService.like(this.$stateParams["id"]).then(response => this.fc.like = response).finally(() => this.disabled = false);
            } else {
                this.flashcardService.likeDelete(this.fc.like).then(() => this.fc.like = null).finally(() => this.disabled = false);
            }
        }
    }

    angular.module("app.flashcard").controller("flashcard", FlashcardController);
}