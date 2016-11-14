var app;
(function (app) {
    "use strict";
    var FitText = (function () {
        function FitText() {
            this.restrict = "A";
            this.link = function (scope, element, attrs) {
                var resizeTextSmall = function () {
                    var elNewFontSize = (parseInt($(element).css('font-size').slice(0, -2)) - 1) + 'px';
                    return $(element).css('font-size', elNewFontSize);
                };
                var resizeTextHeight = function () {
                    var elNewFontSize = (parseInt($(element).css('font-size').slice(0, -2)) + 1) + 'px';
                    return $(element).css('font-size', elNewFontSize);
                };
                scope.$watchGroup([attrs["ngBind"], "f.style"], function (newValue) {
                    $(element).css('font-size', "");
                    if (!newValue) {
                        return;
                    }
                    if (element[0].scrollHeight > element.parent()[0].offsetHeight) {
                        while (element[0].scrollHeight > element.parent()[0].offsetHeight) {
                            resizeTextSmall();
                        }
                    }
                    else {
                        while (element[0].scrollHeight < element.parent()[0].offsetHeight) {
                            resizeTextHeight();
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
