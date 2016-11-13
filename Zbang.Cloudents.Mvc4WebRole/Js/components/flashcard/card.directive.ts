module app {
    "use strict";
    class CardDirective implements angular.IDirective {

        restrict = "E";
        templateUrl = "card-form.html";
        scope: { [boundProperty: string]: string } = {
            slide: "=",
            upload: "="
        }
        link = (scope: angular.IScope) => {
        }
        transclude = true;

        static factory(): angular.IDirectiveFactory {
            const directive = () => {
                return new CardDirective();
            };
            // directive["$inject"] = ["$timeout"];
            return directive;
        }
    }
    angular
        .module("app.flashcard")
        .directive("cardForm", CardDirective.factory());
}