module app {
    "use strict";
    class CardDirective implements angular.IDirective {

        constructor(private $timeout: angular.ITimeoutService) {

        }
        restrict = "E";
        templateUrl = "card-form.html";
        scope: { [boundProperty: string]: string } = {
            slide: "=",
            upload: "=",
            index: "="
        }
        link = (scope: angular.IScope, element: JQuery) => {
            element.on("focusin",
                (e) => {
                    $(".focused").removeClass("focused");
                    element.addClass("focused");
                    element.closest("li").addClass("focused");
                })
                .on("focusout",
                () => {
                    // :focus need some timeout since the first element is document
                    this.$timeout(() => {
                        if ($(":focus").parents(".focused").length) {
                            return;
                        }
                        $(".focused").removeClass("focused");
                    });
                });
        }
        transclude = true;

        static factory(): angular.IDirectiveFactory {
            const directive = ($timeout) => {
                return new CardDirective($timeout);
            };
            directive["$inject"] = ["$timeout"];
            return directive;
        }
    }
    angular
        .module("app.flashcard")
        .directive("cardForm", CardDirective.factory());
}