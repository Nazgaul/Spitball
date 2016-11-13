﻿declare var mOxie: any;
module app {
    "use strict";

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
        front = new CardSlide();
        cover = new CardSlide();

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
        image: string;
        deserialize(input: CardSlide) {
            this.text = input.text;
            this.image = input.image;// || "http://lorempixel.com/400/200/";
            return this;
        }
        uploadProgress: number;

    }

    class FlashcardCreateController {
        data: FlashCard;
        form: angular.IFormController;
        static $inject = ["flashcardService", "$stateParams", "$state", "flashcard", "$scope",
            "$timeout", "$window", "resManager", "$mdDialog", "$q"];
        constructor(private flashcardService: IFlashcardService,
            private $stateParams: spitaball.ISpitballStateParamsService,
            private $state: angular.ui.IStateService,
            private flashcard: FlashCard,
            private $scope: angular.IScope,
            private $timeout: angular.ITimeoutService,
            private $window: angular.IWindowService,
            private resManager: IResManager,
            private $mdDialog: angular.material.IDialogService,
            private $q: angular.IQService) {
            if (flashcard) {
                this.data = new FlashCard().deserialize(flashcard);
                this.data.id = $stateParams["id"];
            } else {
                this.data = new FlashCard();
            }
            
            $scope.$on("update-model",
                () => {
                    this.create();
                });
            $window.onbeforeunload = () => {
                if (this.form.$dirty) {
                    return this.resManager.get("flashcardLeaveTitle");
                }
            };
            $scope.$on("$destroy", () => {
                $window.onbeforeunload = undefined;
            });
            $scope.$on("$stateChangeStart",
                (event: angular.IAngularEvent) => {
                    if (this.form.$dirty) {
                        if (!confirm(this.resManager.get("flashcardLeaveTitle"))) {
                            event.preventDefault();
                            $scope.$emit("state-change-start-prevent");
                        }
                    }

                });
        }
        private serviceCalled = false;
        publish() {
            this.flashcardService.publish(this.data.id, this.data, this.$stateParams.boxId)
                .then(this.navigateBackToBox);
        }
        private navigateBackToBox() {
            this.$state.go("box.flashcards",
                {
                    boxtype: this.$stateParams["boxtype"],
                    universityType: this.$stateParams["universityType"],
                    boxId: this.$stateParams.boxId,
                    boxName: this.$stateParams["boxName"]
                });
        }
        create() {
            const self = this;
            function afterCall() {
                self.serviceCalled = false;
                self.form.$setPristine();
            }
            if (!this.form.$dirty) {
                return this.$q.when();
            }
            if (this.serviceCalled) {
                return this.$q.when();
            }
            this.serviceCalled = true;
            if (this.data.id) {
                return this.flashcardService.update(this.data.id, this.data, this.$stateParams.boxId).then(afterCall);
            }
            return this.flashcardService.create(this.data, this.$stateParams.boxId).then(response => {
                this.data.id = response;
                afterCall();
                this.$state.go("flashcardCreate",
                    {
                        boxtype: this.$stateParams["boxtype"],
                        universityType: this.$stateParams["universityType"],
                        boxId: this.$stateParams.boxId,
                        boxName: this.$stateParams["boxName"],
                        id: response
                    });
            });
        }
        flip() {
            this.data.flip();
        }
        removeCard(index) {
            this.data.cards.splice(index, 1);

        }
        add($index: number) {
            if (angular.isNumber($index)) {
                this.data.cards.splice($index + 1, 0, new Card());
                return;
            }
            this.data.cards.push(new Card());
        }
        close(ev) {
            if (!this.data.id) {
                this.navigateBackToBox();
                return;
            }
            
            const confirm = this.$mdDialog.confirm()
                .title(this.resManager.get('quizLeaveTitle'))
                .textContent(this.resManager.get('quizLeaveContent'))
                .targetEvent(ev)
                .ok(this.resManager.get('quizDelete'))
                .cancel(this.resManager.get('quizSaveAsDraft'));

            this.$mdDialog.show(confirm).then(() => {
                // this.quizService.deleteQuiz(quizId).then(this.navigateBackToBox);
            }, () => {
                this.create().then(this.navigateBackToBox);
            });
        }
        move(dropCardIndex: number, card: Card) {
            const cardIndex = this.data.cards.indexOf(card);
            this.data.cards.splice(cardIndex, 0, this.data.cards.splice(dropCardIndex, 1)[0]);
        }

        upload = {
            url: "/flashcard/image/",
            removeImage: (slide: CardSlide) => {
                this.flashcardService.deleteImage(this.data.id, slide.image);
                slide.image = null;
                this.form.$setDirty();
                this.create();
            },
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
                                slide.image = this.getAsDataURL(file.type, 80);
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
                        //    uploader.start();
                    }, 1);
                },
                error: (uploader, error: plupload_error) => {
                    if (error.code === plupload.FILE_EXTENSION_ERROR) {
                        (this.$scope["app"] as IAppController).showToaster("file error");
                    }
                },
                uploadProgress: (uploader, file) => {
                    (uploader.settings.slide as CardSlide).uploadProgress = file.percent;
                },
                fileUploaded: (uploader, file, response) => {
                    (uploader.settings.slide as CardSlide).uploadProgress = null;
                    var obj = JSON.parse(response.response);
                    if (obj.success) {
                        (uploader.settings.slide as CardSlide).image = obj.payload;
                        this.form.$setDirty();
                        this.create();
                    }
                }
            }
        }
    }
    angular.module("app.flashcard").controller("flashcardCreate", FlashcardCreateController);
}