var app;
(function (app) {
    "use strict";
    var CardSettingsDirective = (function () {
        function CardSettingsDirective($mdMedia) {
            var _this = this;
            this.$mdMedia = $mdMedia;
            this.restrict = "A";
            this.link = function (scope, element, attrs) {
                element.on("click", function () {
                    var selector = ".card-settings";
                    var settings = element.closest("li").find(selector);
                    $(selector).not(settings).removeClass("active");
                    if (_this.$mdMedia(attrs["cardSettings"])) {
                        settings.toggleClass("active");
                    }
                });
            };
        }
        CardSettingsDirective.factory = function () {
            var directive = function ($mdMedia) {
                return new CardSettingsDirective($mdMedia);
            };
            directive["$inject"] = ["$mdMedia"];
            return directive;
        };
        return CardSettingsDirective;
    }());
    angular
        .module("app.flashcard")
        .directive("cardSettings", CardSettingsDirective.factory());
})(app || (app = {}));
