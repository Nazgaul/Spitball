var app;
(function (app) {
    "use strict";
    var CardDirective = (function () {
        function CardDirective() {
            this.restrict = "E";
            this.templateUrl = "card-form.html";
            this.scope = {
                slide: "="
            };
        }
        CardDirective.factory = function () {
            var directive = function () {
                return new CardDirective();
            };
            return directive;
        };
        return CardDirective;
    }());
    angular
        .module("app.flashcard")
        .directive("cardForm", CardDirective.factory());
    var FlashCard = (function () {
        function FlashCard() {
            this.cards = [new Card(), new Card(), new Card(), new Card(), new Card()];
        }
        FlashCard.prototype.deserialize = function (input) {
            this.cards = [];
            this.id = input.id;
            this.name = input.name;
            for (var i = 0; i < input.cards.length; i++) {
                this.cards.push(new Card().deserialize(input.cards[i]));
            }
            if (!this.cards.length) {
                this.cards.push(new Card(), new Card(), new Card(), new Card(), new Card());
            }
            return this;
        };
        FlashCard.prototype.flip = function () {
            this.cards.forEach(function (f) { return f.flip(); });
        };
        return FlashCard;
    }());
    var Card = (function () {
        function Card() {
        }
        Card.prototype.flip = function () {
            var temp = this.front;
            this.front = this.cover;
            this.cover = temp;
        };
        Card.prototype.deserialize = function (input) {
            this.front = input.front;
            this.cover = input.cover;
            return this;
        };
        return Card;
    }());
    var CardSlide = (function () {
        function CardSlide() {
        }
        CardSlide.prototype.deserialize = function (input) {
            this.text = input.text;
            this.img = input.img;
            return this;
        };
        return CardSlide;
    }());
    var FlashcardCreateController = (function () {
        function FlashcardCreateController(flashcardService, $stateParams, $state, flashcard) {
            this.flashcardService = flashcardService;
            this.$stateParams = $stateParams;
            this.$state = $state;
            this.flashcard = flashcard;
            if (flashcard) {
                this.data = new FlashCard().deserialize(flashcard);
            }
            else {
                this.data = new FlashCard();
            }
        }
        FlashcardCreateController.prototype.create = function () {
            var _this = this;
            var self = this;
            this.flashcardService.create(this.data, this.$stateParams.boxId).then(function (response) {
                _this.data.id = response;
                _this.$state.go("flashcardCreate", {
                    boxtype: self.$stateParams["boxtype"],
                    universityType: self.$stateParams["universityType"],
                    boxId: self.$stateParams.boxId,
                    boxName: self.$stateParams["boxName"],
                    id: response
                });
            });
        };
        FlashcardCreateController.prototype.flip = function () {
            this.data.flip();
        };
        FlashcardCreateController.prototype.add = function ($index) {
            if (angular.isNumber($index)) {
                this.data.cards.splice($index + 1, 0, new Card());
                return;
            }
            this.data.cards.push(new Card());
        };
        FlashcardCreateController.prototype.move = function (dropCardIndex, card) {
            var cardIndex = this.data.cards.indexOf(card);
            this.data.cards.splice(cardIndex, 0, this.data.cards.splice(dropCardIndex, 1)[0]);
        };
        FlashcardCreateController.$inject = ["flashcardService", "$stateParams", "$state", "flashcard"];
        return FlashcardCreateController;
    }());
    angular.module("app.flashcard").controller("flashcardCreate", FlashcardCreateController);
})(app || (app = {}));
