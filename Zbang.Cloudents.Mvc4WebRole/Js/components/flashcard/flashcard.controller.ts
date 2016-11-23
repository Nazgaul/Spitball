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
    }
    class Card {
        front = new CardSlide();
        cover = new CardSlide();
        index: number;
        pin: boolean;
        style: boolean = true;
    }
    class CardSlide {
        text: string;
        image: string;

    }
    export class FlashcardController {
        static $inject = ["flashcard", "flashcardService", "$stateParams", "user", "$state", "$mdMedia"];
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
                    if (flashcard.pins && flashcard.pins.indexOf(k) !== -1) {
                        v.pin = true;
                    }
                    v.index = k;
                    if (!v.front.text && !v.front.image) {
                        v.front.text = "...";
                    }
                    if (!v.cover.text && !v.cover.image) {
                        v.cover.text = "...";
                    }
                    v.style = true;

                });
            this.notMobile = $mdMedia("gt-xs");
            this.flashcard = flashcard;
            flashcard.pins = flashcard.pins || [];
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
            this.flashcardService.solve(this.$stateParams["id"]);
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
        }
        next() {
            this.slidepos = Math.min(this.cards.length, ++this.slidepos);
            if (this.slidepos === this.cards.length) {
                this.step = Steps.End;
                return;
            }
        }
        changeLegend(legend?: boolean) {
            this.styleLegend = legend;
            angular.forEach(this.cards,
                (c => {
                    c.style = this.styleLegend;
                }));
        }
        flip(slide) {
            if (typeof (slide.style) === "boolean") {
                slide.style = !slide.style;
            }

        }
        pin(slide: Card) {
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
            if (!this.canLike()) {
                return;
            }
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