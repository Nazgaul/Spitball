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
                    var change = isPositive ? 1 : -1, currentFontSize = parseInt($(element).css(attributeToChange).slice(0, -2));
                    if (currentFontSize <= 18) {
                        return false;
                    }
                    var elNewFontSize = (currentFontSize + change) + 'px';
                    $(element).css(attributeToChange, elNewFontSize);
                    return true;
                };
                var d = scope.$watchGroup([attrs["ngBind"], "f.styleLegend", "f.slidepos"], function (newValue) {
                    _this.$animate.removeClass(element.parents("angular-animate"), "ng-hide")
                        .then(function () {
                        changeFont();
                    });
                    _this.$animate.removeClass(element.parents("angular-animate"), "front back both")
                        .then(function () {
                        changeFont();
                    });
                    changeFont();
                    function changeFont() {
                        $(element).css(attributeToChange, "").removeClass("lessText");
                        if (!newValue[0]) {
                            return;
                        }
                        if (element.is(":hidden")) {
                            return;
                        }
                        if (newValue[0].length <= 80) {
                            element.addClass("lessText");
                        }
                        while (element[0].scrollHeight > element.parent().height() ||
                            element[0].scrollWidth > element.parent().width()) {
                            if (!changeFontSize(false)) {
                                break;
                            }
                        }
                    }
                });
                scope.$on("$destroy", function () {
                    d();
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
//# sourceMappingURL=fitText.directive.js.map