declare var mOxie: any;
module app {

    "use strict";

    class CardDirective implements angular.IDirective {

        restrict = "E";
        templateUrl = "card-form.html";
        scope: { [boundProperty: string]: string } = {
            slide: "=",
            upload: "="
        }
        link = (scope: angular.IScope) => {
        }
        transclude = true;

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
            this.cards = [];
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
            this.front = new CardSlide().deserialize(input.front);
            this.cover = new CardSlide().deserialize(input.cover);
            return this;
        }
    }
    class CardSlide implements ISerializable<CardSlide> {
        text: string;
        img: string;
        deserialize(input: CardSlide) {
            this.text = input.text;
            this.img = input.img;// || "http://lorempixel.com/400/200/";
            return this;
        }
        uploadProgress: number;

    }

    class FlashcardCreateController {
        data: FlashCard;
        static $inject = ["flashcardService", "$stateParams", "$state", "flashcard", "$scope","$timeout"];
        constructor(private flashcardService: IFlashcardService,
            private $stateParams: spitaball.ISpitballStateParamsService,
            private $state: angular.ui.IStateService,
            private flashcard: FlashCard,
            private $scope: angular.IScope,
            private $timeout: angular.ITimeoutService) {

            if (flashcard) {
                this.data = new FlashCard().deserialize(flashcard);
            } else {
                this.data = new FlashCard();
            }

        }
        create() {
            const self = this;
            this.flashcardService.create(this.data, this.$stateParams.boxId).then(response => {
                this.data.id = response;

                this.$state.go("flashcardCreate",
                    {
                        boxtype: self.$stateParams["boxtype"],
                        universityType: self.$stateParams["universityType"],
                        boxId: self.$stateParams.boxId,
                        boxName: self.$stateParams["boxName"],
                        id: response
                    });
            });
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

        upload = {
            url: "/upload/flashcardimage/",
            options: (slide) => {
                return {
                    slide: slide,
                    multi_selection: false,
                    filters: {
                        mime_types: [
                            { title: "Image files", extensions: "jpg,gif,png,jpeg" }
                        ]
                    },
                    resize: {
                        preserve_headers: false
                    }
                }
            },
            callbacks: {
                filesAdded: (uploader, files) => {
                    for (let i = 0; i < files.length; i++) {
                        ((file, slide: CardSlide, self: FlashcardCreateController) => {
                            var img = new mOxie.Image();
                            img.onload = function () {
                                this.crop(105, 105, false);
                                slide.img = this.getAsDataURL("image/jpeg", 80);
                                slide.uploadProgress = 50;
                                self.$scope.$apply();

                            };
                            img.onembedded = function () {
                                this.destroy();
                            };

                            img.onerror = function () {
                                this.destroy();
                            };
                            img.load(file.getSource());
                        })(files[i], uploader.settings.slide, this);
                    }
                    this.$timeout(() => {
                        uploader.start();
                    },1);
                },
                error: (uploader, error: plupload_error) => {
                    if (error.code === plupload.FILE_EXTENSION_ERROR) {
                        // TODO: continue

                        alert("file error");
                    }
                },
                uploadProgress: (uploader, file) => {
                    (uploader.settings.slide as CardSlide).uploadProgress = file.percent;
                    //console.log(file);
                },
                fileUploaded: (uploader, file, response) => {
                    (uploader.settings.slide as CardSlide).uploadProgress = null;
                    var obj = JSON.parse(response.response);
                    if (obj.success) {
                        (uploader.settings.slide as CardSlide).img = obj.payload;
                    }
                }
            }
        }
    }
    angular.module("app.flashcard").controller("flashcardCreate", FlashcardCreateController);
}