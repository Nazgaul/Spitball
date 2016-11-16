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