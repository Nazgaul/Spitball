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
                    var resizeOccured;
                    while (element[0].scrollHeight > element.parent()[0].offsetHeight && element[0].scrollWidth > element.parent()[0].offsetWidth) {
                            changeFontSize(false);
                        resizeOccured = true;
                    }
                    if (!resizeOccured) {
                        while (element[0].scrollHeight < element.parent()[0].offsetHeight && element[0].scrollWidth < element.parent()[0].offsetWidth) {
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
