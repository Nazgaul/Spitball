var app;
(function (app) {
    "use strict";
    var CardDirective = (function () {
        function CardDirective($timeout) {
            this.$timeout = $timeout;
            this.restrict = "E";
            this.templateUrl = "card-form.html";
            this.scope = {
                slide: "=",
                upload: "=",
                index: "="
            };
            this.transclude = true;
        }
        CardDirective.factory = function () {
            var directive = function ($timeout) {
                return new CardDirective($timeout);
            };
            directive["$inject"] = ["$timeout"];
            return directive;
        };
        return CardDirective;
    }());
    angular
        .module("app.flashcard")
        .directive("cardForm", CardDirective.factory());
})(app || (app = {}));
