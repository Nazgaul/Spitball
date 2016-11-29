var app;
(function (app) {
    "use strict";
    var Steps;
    (function (Steps) {
        Steps[Steps["Start"] = 0] = "Start";
        Steps[Steps["Memo"] = 1] = "Memo";
        Steps[Steps["End"] = 2] = "End";
    })(Steps || (Steps = {}));
    ;
    function shuffle(a) {
        var j, x, i;
        for (i = a.length; i; i--) {
            j = Math.floor(Math.random() * i);
            x = a[i - 1];
            a[i - 1] = a[j];
            a[j] = x;
        }
    }
    var Flashcard = (function () {
        function Flashcard() {
        }
        return Flashcard;
    }());
    var UniversityData = (function () {
        function UniversityData() {
        }
        return UniversityData;
    }());
    var Card = (function () {
        function Card() {
            this.front = new CardSlide();
            this.cover = new CardSlide();
            this.style = true;
        }
        return Card;
    }());
    var CardSlide = (function () {
        function CardSlide() {
        }
        return CardSlide;
    }());
    var FlashcardController = (function () {
        function FlashcardController(flashcard, flashcardService, $stateParams, user, $state, $mdMedia, $scope, $mdDialog) {
            this.flashcardService = flashcardService;
            this.$stateParams = $stateParams;
            this.user = user;
            this.$state = $state;
            this.$mdMedia = $mdMedia;
            this.$scope = $scope;
            this.$mdDialog = $mdDialog;
            this.step = Steps.Start;
            this.slidepos = 0;
            this.disabled = false;
            this.styleLegend = true;
            this.flipped = false;
            this.pinCount = 0;
            console.log(flashcard);
            angular.forEach(flashcard.cards, function (v, k) {
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
        FlashcardController.prototype.start = function () {
            this.cards = this.flashcard.cards.slice(0);
            this.goToStep2();
        };
        FlashcardController.prototype.goToStep2 = function () {
            if (this.shuffle) {
                shuffle(this.cards);
            }
            this.flashcardService.solve(this.$stateParams["id"]);
            this.slidepos = 0;
            this.step = Steps.Memo;
        };
        FlashcardController.prototype.startPin = function () {
            this.cards = this.flashcard.cards.filter(function (f) { return f.pin; });
            this.goToStep2();
        };
        FlashcardController.prototype.prev = function () {
            this.changeLegend(this.styleLegend);
            this.slidepos = Math.max(0, --this.slidepos);
            this.step = Steps.Memo;
        };
        FlashcardController.prototype.next = function () {
            this.changeLegend(this.styleLegend);
            this.slidepos = Math.min(this.cards.length, ++this.slidepos);
            if (this.slidepos === this.cards.length) {
                this.step = Steps.End;
                return;
            }
        };
        FlashcardController.prototype.changeLegend = function (legend) {
            var _this = this;
            this.styleLegend = legend;
            angular.forEach(this.cards, (function (c) {
                c.style = _this.styleLegend;
            }));
        };
        FlashcardController.prototype.flip = function (slide) {
            if (typeof (slide.style) === "boolean") {
                slide.style = !slide.style;
                this.flipped = !this.flipped;
            }
        };
        FlashcardController.prototype.pin = function (slide) {
            slide.pin = !slide.pin;
            if (slide.pin) {
                this.pinCount++;
                this.flashcardService.pin(this.$stateParams["id"], slide.index);
            }
            else {
                this.pinCount--;
                this.flashcardService.pinDelete(this.$stateParams["id"], slide.index);
            }
        };
        FlashcardController.prototype.like = function () {
            var _this = this;
            if (!this.canLike()) {
                return;
            }
            this.disabled = true;
            if (!this.flashcard.like) {
                this.flashcardService.like(this.$stateParams["id"]).then(function (response) { return _this.flashcard.like = response; }).finally(function () { return _this.disabled = false; });
            }
            else {
                this.flashcardService.likeDelete(this.flashcard.like).then(function () { return _this.flashcard.like = null; }).finally(function () { return _this.disabled = false; });
            }
        };
        FlashcardController.prototype.canLike = function () {
            return this.user.id !== this.flashcard.userId;
        };
        FlashcardController.prototype.details = function (ev) {
            this.$mdDialog.show({
                templateUrl: "/flashcard/promo/",
                targetEvent: ev,
                clickOutsideToClose: true,
                locals: {
                    color1: this.flashcard.universityData.btnColor,
                    color2: this.flashcard.universityData.btnFontColor,
                    university: this.flashcard.universityData.universityName
                },
                controller: "DialogPromo",
                controllerAs: "dp",
                fullscreen: false
            });
        };
        FlashcardController.$inject = ["flashcard", "flashcardService", "$stateParams",
            "user", "$state", "$mdMedia", "$scope", "$mdDialog"];
        return FlashcardController;
    }());
    app.FlashcardController = FlashcardController;
    angular.module("app.flashcard").controller("flashcard", FlashcardController);
})(app || (app = {}));
