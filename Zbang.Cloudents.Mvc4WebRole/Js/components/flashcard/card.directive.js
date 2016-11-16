var app;
(function (app) {
    "use strict";
    var CardDirective = (function () {
        function CardDirective($timeout) {
            var _this = this;
            this.$timeout = $timeout;
            this.restrict = "E";
            this.templateUrl = "card-form.html";
            this.scope = {
                slide: "=",
                upload: "=",
                index: "="
            };
            this.link = function (scope, element) {
                element.on("focusin", function (e) {
                    $(".focused").removeClass("focused");
                    element.addClass("focused");
                    element.closest("li").addClass("focused");
                })
                    .on("focusout", function () {
                    _this.$timeout(function () {
                        if ($(":focus").parents(".focused").length) {
                            return;
                        }
                        $(".focused").removeClass("focused");
                    });
                });
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
