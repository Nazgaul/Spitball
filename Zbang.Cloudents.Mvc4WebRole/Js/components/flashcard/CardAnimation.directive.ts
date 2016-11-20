﻿module app {
    "use strict";
    class CardSlideAnimation implements angular.IDirective {

        constructor(private $animate: angular.animate.IAnimateService) {

        }
        scope: { [boundProperty: string]: string } = {
            'myHide': '=',
            "slidePos": "="
        }
        restrict = "A";
        link = (scope: angular.IScope, element: JQuery, attrs: ng.IAttributes) => {
            var $body = $("body");
            var d = scope.$watchGroup(['myHide', "slidePos"], (newValue, oldValue) => {

                console.log(newValue, oldValue, element.attr("id"));
                element.removeClass("backSlide");
                if (newValue[1] < oldValue[1]) {
                    console.log(newValue, oldValue);
                    element.addClass("backSlide");

                }
                if (!newValue[0]) {
                    $body.css("overflow", "hidden");
                    this.$animate.removeClass(element, "ng-hide")
                        .then(() => {
                            removeOverFlow(newValue[0], oldValue[0]);
                        });
                }
                else {
                    $body.css("overflow", "hidden");
                    if (!element.hasClass("ng-hide")) {
                        this.$animate.addClass(element, "ng-hide")
                            .then(() => {
                                removeOverFlow(newValue[0], oldValue[0]);
                            });
                    }
                }
            });
            function removeOverFlow(newVal,oldVal) {
                if (newVal === oldVal) {
                    return;
                }
                console.log(element);
                $body.css("overflow", "");
                //element.removeClass("backSlide");
            }
            scope.$on("$destroy",
                () => {
                    d();
                    $body.css("overflow", "");
                });
        }
        static factory(): angular.IDirectiveFactory {
            const directive = ($animate) => {
                return new CardSlideAnimation($animate);
            };
            directive["$inject"] = ["$animate"];
            return directive;
        }
    }
    angular
        .module("app.flashcard")
        .directive("myHide", CardSlideAnimation.factory());
}