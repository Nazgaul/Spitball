module app {
    "use strict";

    class CardDirective implements angular.IDirective {

        restrict = "E";
        templateUrl = "card-form.html";
        scope: { [boundProperty: string]: string } = {
            slide: "="
        }

        static factory(): angular.IDirectiveFactory {
            const directive = () => {
                return new CardDirective();
            };
            // directive["$inject"] = ["$timeout"];
            return directive;
        }
    }
    angular
        .module("app.flashcard")
        .directive("cardForm", CardDirective.factory());

    class FlashCard implements ISerializable<FlashCard> {

        id: number;
        name: string;
        cards: Array<Card> = [new Card(), new Card(), new Card(), new Card(), new Card()];
        deserialize(input: FlashCard) {
            this.id = input.id;
            this.name = input.name;
            for (let i = 0; i < input.cards.length; i++) {
                this.cards.push(new Card().deserialize(input.cards[i]));
            }
            if (!this.cards.length) {
                this.cards.push(new Card(), new Card(), new Card(), new Card(), new Card());
            }
            return this;
        }
        flip() {
            this.cards.forEach(f => f.flip());
        }


    }
    class Card implements ISerializable<Card> {
        front: CardSlide;
        cover: CardSlide;
        flip() {
            const temp = this.front;
            this.front = this.cover;
            this.cover = temp;
        }
        deserialize(input: Card) {
            this.front = input.front;
            this.cover = input.cover;
            return this;
        }
    }
    class CardSlide implements ISerializable<CardSlide> {
        text: string;
        img: string;

        deserialize(input: CardSlide) {
            this.text = input.text;
            this.img = input.img;
            return this;
        }
    }

    class FlashcardCreateController {
        data: FlashCard;
        constructor() {
            this.data = new FlashCard();
        }
        create() {
            console.log(this.data);
        }
        flip() {
            this.data.flip();
        }
        add($index: number) {
            if (angular.isNumber($index)) {
                this.data.cards.splice($index + 1, 0, new Card());
                return;
            }
            this.data.cards.push(new Card());
        }
        move(dropCardIndex: number, card: Card) {
            const cardIndex = this.data.cards.indexOf(card);
            this.data.cards.splice(cardIndex, 0, this.data.cards.splice(dropCardIndex, 1)[0]);
        }
    }
    angular.module("app.flashcard").controller("flashcardCreate", FlashcardCreateController);
}