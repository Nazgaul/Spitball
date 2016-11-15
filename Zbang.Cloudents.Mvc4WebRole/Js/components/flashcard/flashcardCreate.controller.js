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
            input.cards = input.cards || [];
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
    app.FlashCard = FlashCard;
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
    app.Card = Card;
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
    app.CardSlide = CardSlide;
    var FlashcardCreateController = (function () {
        function FlashcardCreateController(flashcardService, $stateParams, $state, flashcard, $scope, $timeout, $window, resManager, $mdDialog, $q) {
            var _this = this;
            this.flashcardService = flashcardService;
            this.$stateParams = $stateParams;
            this.$state = $state;
            this.flashcard = flashcard;
            this.$scope = $scope;
            this.$timeout = $timeout;
            this.$window = $window;
            this.resManager = resManager;
            this.$mdDialog = $mdDialog;
            this.$q = $q;
            this.disabled = false;
            this.serviceCalled = false;
            this.navigateBackToBox = function () {
                _this.form.$setPristine();
                _this.$state.go("box.flashcards", {
                    boxtype: _this.$stateParams["boxtype"],
                    universityType: _this.$stateParams["universityType"],
                    boxId: _this.$stateParams.boxId,
                    boxName: _this.$stateParams["boxName"]
                });
            };
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
                            preserve_headers: false,
                            width: 350,
                            height: 350
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
                            _this.disabled = true;
                            uploader.start();
                        }, 1);
                    },
                    error: function (uploader, error) {
                        if (error.code === plupload.FILE_EXTENSION_ERROR) {
                            _this.$scope["app"].showToaster("file error");
                        }
                    },
                    UploadComplete: function () {
                        _this.disabled = false;
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
            $window.onbeforeunload = function () {
                if (_this.form.$dirty) {
                    return _this.resManager.get("flashcardLeaveTitle");
                }
            };
            $scope.$on("$destroy", function () {
                $window.onbeforeunload = undefined;
            });
            $scope.$on("$stateChangeStart", function (event) {
                if (_this.form.$dirty) {
                    if (!confirm(_this.resManager.get("flashcardLeaveTitle"))) {
                        event.preventDefault();
                        $scope.$emit("state-change-start-prevent");
                    }
                }
            });
        }
        FlashcardCreateController.prototype.publish = function () {
            var self = this;
            if (this.form.$invalid) {
                if (this.form["name"].$invalid) {
                    this.form["name"].$setTouched();
                }
                return;
            }
            else {
                if (!this.data.id) {
                    this.create().then(publish2);
                    return;
                }
                publish2();
            }
            function publish2() {
                self.flashcardService.publish(self.data.id, self.data, self.$stateParams.boxId)
                    .then(self.navigateBackToBox);
            }
        };
        FlashcardCreateController.prototype.create = function () {
            var _this = this;
            var self = this;
            function afterCall() {
                self.serviceCalled = false;
                self.disabled = false;
                self.form.$setPristine();
            }
            if (!this.form.$dirty) {
                return this.$q.when();
            }
            if (this.serviceCalled) {
                return this.$q.when();
            }
            this.disabled = true;
            this.serviceCalled = true;
            if (this.data.id) {
                return this.flashcardService.update(this.data.id, this.data, this.$stateParams.boxId).then(afterCall);
            }
            return this.flashcardService.create(this.data, this.$stateParams.boxId).then(function (response) {
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
        FlashcardCreateController.prototype.close = function (ev) {
            var _this = this;
            if (!this.data.id && !this.form.$dirty) {
                this.navigateBackToBox();
                return;
            }
            var confirm = this.$mdDialog.confirm()
                .title(this.resManager.get('flashcardLeaveTitle'))
                .textContent(this.resManager.get('flashcardLeaveContent'))
                .targetEvent(ev)
                .ok(this.resManager.get('quizDelete'))
                .cancel(this.resManager.get('quizSaveAsDraft'));
            this.$mdDialog.show(confirm).then(function () {
                if (_this.data.id) {
                    _this.flashcardService.delete(_this.data.id).then(_this.navigateBackToBox);
                }
                else {
                    _this.navigateBackToBox();
                }
            }, function () {
                _this.create().then(_this.navigateBackToBox);
            });
        };
        FlashcardCreateController.prototype.move = function (dropCardIndex, card) {
            var cardIndex = this.data.cards.indexOf(card);
            this.data.cards.splice(cardIndex, 0, this.data.cards.splice(dropCardIndex, 1)[0]);
        };
        FlashcardCreateController.$inject = ["flashcardService", "$stateParams", "$state", "flashcard", "$scope",
            "$timeout", "$window", "resManager", "$mdDialog", "$q"];
        return FlashcardCreateController;
    }());
    angular.module("app.flashcard").controller("flashcardCreate", FlashcardCreateController);
})(app || (app = {}));
