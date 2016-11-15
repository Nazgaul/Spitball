var app;
(function (app) {
    "use strict";
    var CardDirective = (function () {
        function CardDirective() {
            this.restrict = "E";
            this.templateUrl = "card-form.html";
            this.scope = {
                slide: "=",
                upload: "="
            };
            this.transclude = true;
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
})(app || (app = {}));
