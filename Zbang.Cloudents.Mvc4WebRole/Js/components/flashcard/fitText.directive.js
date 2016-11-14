var app;
(function (app) {
    "use strict";
    var FitText = (function () {
        function FitText() {
            this.restrict = "A";
            this.link = function (scope, element, attrs) {
                var attributeToChange = "font-size";
                var changeFontSize = function (isPositive) {
                    var change = isPositive ? 1 : -1;
                    var elNewFontSize = (parseInt($(element).css(attributeToChange).slice(0, -2)) + change) + 'px';
                    return $(element).css(attributeToChange, elNewFontSize);
                };
                scope.$watchGroup([attrs["ngBind"], "f.style"], function (newValue) {
                    $(element).css(attributeToChange, "");
                    if (!newValue[0]) {
                        return;
                    }
                    if (element[0].scrollHeight > element.parent()[0].offsetHeight) {
                        while (element[0].scrollHeight > element.parent()[0].offsetHeight) {
                            changeFontSize(false);
                        }
                    }
                    else {
                        while (element[0].scrollHeight < element.parent()[0].offsetHeight) {
                            changeFontSize(true);
                        }
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
