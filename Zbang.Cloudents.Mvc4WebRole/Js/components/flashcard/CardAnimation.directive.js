var app;
(function (app) {
    "use strict";
    var CardSlideAnimation = (function () {
        function CardSlideAnimation($animate) {
            var _this = this;
            this.$animate = $animate;
            this.scope = {
                'myHide': '=',
                "slidePos": "="
            };
            this.restrict = "A";
            this.link = function (scope, element) {
                var $body = $("body");
                var d = scope.$watchGroup(['myHide', "slidePos"], function (newValue, oldValue) {
                    element.removeClass("backSlide");
                    if (newValue[1] < oldValue[1]) {
                        element.addClass("backSlide");
                    }
                    if (!newValue[0]) {
                        $body.css("overflow", "hidden");
                        _this.$animate.removeClass(element, "ng-hide")
                            .then(function () {
                            removeOverFlow(newValue[0], oldValue[0]);
                        });
                    }
                    else {
                        if (!element.hasClass("ng-hide")) {
                            $body.css("overflow", "hidden");
                            _this.$animate.addClass(element, "ng-hide")
                                .then(function () {
                                removeOverFlow(newValue[0], oldValue[0]);
                            });
                        }
                    }
                });
                function removeOverFlow(newVal, oldVal) {
                    $body.css("overflow", "");
                }
                scope.$on("$destroy", function () {
                    d();
                    $body.css("overflow", "");
                });
            };
        }
        CardSlideAnimation.factory = function () {
            var directive = function ($animate) {
                return new CardSlideAnimation($animate);
            };
            directive["$inject"] = ["$animate"];
            return directive;
        };
        return CardSlideAnimation;
    }());
    angular
        .module("app.flashcard")
        .directive("myHide", CardSlideAnimation.factory());
})(app || (app = {}));
