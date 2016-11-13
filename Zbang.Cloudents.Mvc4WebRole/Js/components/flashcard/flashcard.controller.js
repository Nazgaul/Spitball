var app;
(function (app) {
    "use strict";
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
            this.front = new CardSlide();
            this.cover = new CardSlide();
        }
        Card.prototype.flip = function () {
            var temp = this.front;
            this.front = this.cover;
            this.cover = temp;
        };
        Card.prototype.deserialize = function (input) {
            this.front = new CardSlide().deserialize(input.front);
            this.cover = new CardSlide().deserialize(input.cover);
            return this;
        };
        return Card;
    }());
    var CardSlide = (function () {
        function CardSlide() {
        }
        CardSlide.prototype.deserialize = function (input) {
            this.text = input.text;
            this.image = input.image;
            return this;
        };
        return CardSlide;
    }());
    var FlashcardCreateController = (function () {
        function FlashcardCreateController(flashcardService, $stateParams, $state, flashcard, $scope, $timeout) {
            var _this = this;
            this.flashcardService = flashcardService;
            this.$stateParams = $stateParams;
            this.$state = $state;
            this.flashcard = flashcard;
            this.$scope = $scope;
            this.$timeout = $timeout;
            this.serviceCalled = false;
            this.upload = {
                url: "/flashcard/image/",
                removeImage: function (slide) {
                    _this.flashcardService.deleteImage(_this.data.id, slide.image);
                    slide.image = null;
                    _this.form.$setDirty();
                    _this.create();
                },
                options: function (slide) {
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
                    };
                },
                callbacks: {
                    filesAdded: function (uploader, files) {
                        for (var i = 0; i < files.length; i++) {
                            (function (file, slide, self) {
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
                            })(files[i], uploader.settings.slide, _this);
                        }
                        _this.$timeout(function () {
                        }, 1);
                    },
                    error: function (uploader, error) {
                        if (error.code === plupload.FILE_EXTENSION_ERROR) {
                            _this.$scope["app"].showToaster("file error");
                        }
                    },
                    uploadProgress: function (uploader, file) {
                        uploader.settings.slide.uploadProgress = file.percent;
                    },
                    fileUploaded: function (uploader, file, response) {
                        uploader.settings.slide.uploadProgress = null;
                        var obj = JSON.parse(response.response);
                        if (obj.success) {
                            uploader.settings.slide.image = obj.payload;
                            _this.form.$setDirty();
                            _this.create();
                        }
                    }
                }
            };
            if (flashcard) {
                this.data = new FlashCard().deserialize(flashcard);
                this.data.id = $stateParams["id"];
            }
            else {
                this.data = new FlashCard();
            }
            $scope.$on("update-model", function () {
                _this.create();
            });
        }
        FlashcardCreateController.prototype.publish = function () {
            var _this = this;
            this.flashcardService.publish(this.data.id, this.data, this.$stateParams.boxId)
                .then(function () {
                _this.$state.go("box.flashcards", {
                    boxtype: _this.$stateParams["boxtype"],
                    universityType: _this.$stateParams["universityType"],
                    boxId: _this.$stateParams.boxId,
                    boxName: _this.$stateParams["boxName"]
                });
            });
        };
        FlashcardCreateController.prototype.create = function () {
            var _this = this;
            var self = this;
            function afterCall() {
                self.serviceCalled = false;
                self.form.$setPristine();
            }
            if (!this.form.$dirty) {
                return;
            }
            if (this.serviceCalled) {
                return;
            }
            this.serviceCalled = true;
            if (this.data.id) {
                this.flashcardService.update(this.data.id, this.data, this.$stateParams.boxId).then(afterCall);
                return;
            }
            this.flashcardService.create(this.data, this.$stateParams.boxId).then(function (response) {
                _this.data.id = response;
                afterCall();
                _this.$state.go("flashcardCreate", {
                    boxtype: _this.$stateParams["boxtype"],
                    universityType: _this.$stateParams["universityType"],
                    boxId: _this.$stateParams.boxId,
                    boxName: _this.$stateParams["boxName"],
                    id: response
                });
            });
        };
        FlashcardCreateController.prototype.flip = function () {
            this.data.flip();
        };
        FlashcardCreateController.prototype.removeCard = function (index) {
            this.data.cards.splice(index, 1);
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
        FlashcardCreateController.$inject = ["flashcardService", "$stateParams", "$state", "flashcard", "$scope", "$timeout"];
        return FlashcardCreateController;
    }());
    angular.module("app.flashcard").controller("flashcardCreate", FlashcardCreateController);
})(app || (app = {}));
