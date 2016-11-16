var app;
(function (app) {
    "use strict";
    var FitText = (function () {
        function FitText($animate) {
            var _this = this;
            this.$animate = $animate;
            this.restrict = "A";
            this.link = function (scope, element, attrs) {
                var attributeToChange = "font-size";
                var changeFontSize = function (isPositive) {
                    var change = isPositive ? 1 : -1;
                    var elNewFontSize = (parseInt($(element).css(attributeToChange).slice(0, -2)) + change) + 'px';
                    return $(element).css(attributeToChange, elNewFontSize);
                };
                scope.$watchGroup([attrs["ngBind"], "f.style", "f.slidepos"], function (newValue) {
                    _this.$animate.removeClass(element.parents("angular-animate"), "ng-hide")
                        .then(function () {
                        changeFont();
                    });
                    _this.$animate.removeClass(element.parents("angular-animate"), "front back both")
                        .then(function () {
                        console.log("x");
                    });
                    changeFont();
                    function changeFont() {
                        $(element).css(attributeToChange, "");
                        if (!newValue[0]) {
                            return;
                        }
                        if (element.parents(".ng-hide").length) {
                            return;
                        }
                        var resizeOccured = false;
                        while (element[0].scrollHeight > element.parent()[0].offsetHeight ||
                            element[0].scrollWidth > element.parent()[0].offsetWidth) {
                            changeFontSize(false);
                            resizeOccured = true;
                        }
                        if (!resizeOccured) {
                            while (element[0].scrollHeight < element.parent()[0].offsetHeight &&
                                element[0].scrollWidth < element.parent()[0].offsetWidth) {
                                changeFontSize(true);
                            }
                        }
                    }
                });
            };
        }
        FitText.factory = function () {
            var directive = function ($animate) {
                return new FitText($animate);
            };
            directive["$inject"] = ["$animate"];
            return directive;
        };
        return FitText;
    }());
    angular
        .module("app.flashcard")
        .directive("fitText", FitText.factory());
})(app || (app = {}));
