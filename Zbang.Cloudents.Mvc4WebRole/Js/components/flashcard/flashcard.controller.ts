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
        userId: number;
        ownerName: string;

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
    class Card  {
        front = new CardSlide();
        cover = new CardSlide();
        index: number;
        pin: boolean;
        style: boolean = true;
        //deserialize(input: Card) {
        //    this.front = new CardSlide().deserialize(input.front);
        //    this.cover = new CardSlide().deserialize(input.cover);
        //    return this;
        //}
    }
    class CardSlide {
        text: string;
        image: string;
        //deserialize(input: CardSlide) {
        //    this.text = input.text;
        //    this.image = input.image;
        //    return this;
        //}

    }
    export class FlashcardController {
        static $inject = ["flashcard", "flashcardService", "$stateParams", "user", "$state","$mdMedia"];
        cards: Array<Card>;
        flashcard: Flashcard;
        shuffle: boolean;
        step = Steps.Start;
        slidepos = 0;
        //slide: Card;
        disabled = false;
        styleLegend = true;
        //style = true;
        backUrl;
        notMobile: boolean;

        pinCount = 0;
        constructor(flashcard: Flashcard,
            private flashcardService: IFlashcardService,
            private $stateParams: angular.ui.IStateParamsService,
            private user: IUserData,
            private $state: angular.ui.IStateService,
            private $mdMedia: angular.material.IMedia) {
            angular.forEach(flashcard.cards,
                (v, k) => {
                    if (flashcard.pins.indexOf(k) !== -1) {
                        v.pin = true;
                    }
                    v.index = k;
                    if (!v.front.text) {
                        v.front.text = "...";
                    }
                    if (!v.cover.text) {
                        v.cover.text = "...";
                    }
                    v.style = true;

                });
            this.notMobile = $mdMedia("gt-xs");
            this.flashcard = flashcard;
            this.pinCount = flashcard.pins.length;
            this.backUrl = $state.href("box.flashcards", angular.extend({}, $stateParams, { boxtype: "course" }));
        }

        start() {
            this.cards = this.flashcard.cards.slice(0);
            this.goToStep2();
        }
        private goToStep2() {
            if (this.shuffle) {
                shuffle(this.cards);
            }
            this.slidepos = 0;
            // this.slide = this.cards[this.slidepos];
            this.step = Steps.Memo;
        }
        startPin() {
            this.cards = this.flashcard.cards.filter(f => f.pin);
            this.goToStep2();
        }
        prev() {
            this.slidepos = Math.max(0, --this.slidepos);
            this.step = Steps.Memo;
            //console.log(this.style, this.styleLegend);
            //this.style = this.styleLegend;
           
        }
        next() {
            this.slidepos = Math.min(this.cards.length, ++this.slidepos);
            if (this.slidepos === this.cards.length) {
                this.step = Steps.End;
                return;
            }
            //console.log(this.style, this.styleLegend);
            //this.style = this.styleLegend;
        }
        changeLegend(legend?: boolean) {
            this.styleLegend = legend;
            //this.style = legend;
            angular.forEach(this.cards,
                (c => {
                    c.style = this.styleLegend;
                }));
        }
        flip(slide) {
            slide.style = !slide.style;
            //if (this.style !== null) {
            //    this.style = !this.style;
            //}
        }
        //changeStyle(s) {
        //    this.style = this.styleLegend = s;
        //}
        pin(slide: Card) {
            //const index = this.flashcard.cards.indexOf(this.slide);
            //console.log(this.slide, index);
            slide.pin = !slide.pin;
            if (slide.pin) {
                this.pinCount++;
                this.flashcardService.pin(this.$stateParams["id"], slide.index);
            } else {
                this.pinCount--;
                this.flashcardService.pinDelete(this.$stateParams["id"], slide.index);
            }
        }
        like() {
            this.disabled = true;
            if (!this.flashcard.like) {
                this.flashcardService.like(this.$stateParams["id"]).then(response => this.flashcard.like = response).finally(() => this.disabled = false);
            } else {
                this.flashcardService.likeDelete(this.flashcard.like).then(() => this.flashcard.like = null).finally(() => this.disabled = false);
            }
        }
        canLike() {
            return this.user.id !== this.flashcard.userId;
        }
    }

    angular.module("app.flashcard").controller("flashcard", FlashcardController);
}