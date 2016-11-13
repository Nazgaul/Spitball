var app;
(function (app) {
    var CardSettingsDirective = (function () {
        function CardSettingsDirective($mdMedia) {
            var _this = this;
            this.$mdMedia = $mdMedia;
            this.restrict = "A";
            this.link = function (scope, element, attrs) {
                var allCardsSettings = $(".card-settings");
                var currentCardSettings = $(attrs["cardSettings"]);
                element.on(attrs["cardSettingsEvent"], function () {
                    if (!_this.$mdMedia('xs')) {
                        allCardsSettings.hide();
                        currentCardSettings.show();
                    }
                });
            };
        }
        CardSettingsDirective.factory = function () {
            var directive = function ($mdMedia) {
                return new CardSettingsDirective($mdMedia);
            };
            directive['$inject'] = ['$mdMedia'];
            return directive;
        };
        return CardSettingsDirective;
    }());
    angular
        .module("app.flashcard")
        .directive("cardSettings", CardSettingsDirective.factory());
})(app || (app = {}));
