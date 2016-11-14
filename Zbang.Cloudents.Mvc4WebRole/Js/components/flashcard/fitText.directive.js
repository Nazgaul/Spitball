var app;
(function (app) {
    "use strict";
    var FitText = (function () {
        function FitText() {
            this.restrict = "A";
            this.link = function (scope, element, attrs) {
                var resizeText = function () {
                    var elNewFontSize = (parseInt($(element).css('font-size').slice(0, -2)) - 1) + 'px';
                    return $(element).css('font-size', elNewFontSize);
                };
                scope.$watch(attrs["ngBind"], function () {
                    while (element[0].scrollHeight > element[0].offsetHeight) {
                        resizeText();
                    }
                });
            };
        }
        FitText.factory = function () {
            var directive = function () {
                return new FitText();
            };
            return directive;
        };
        return FitText;
    }());
    angular
        .module("app.flashcard")
        .directive("fitText", FitText.factory());
})(app || (app = {}));
