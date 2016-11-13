module app {
    "use strict";
    class InteractionDone implements angular.IDirective {

        constructor(private $timeout: angular.ITimeoutService) {

        }
        restrict = "A";
        scope = true;

        link = (scope: angular.IScope, element: JQuery) => {
            var x;
            element.on("click keydown",
                () => {
                    this.$timeout.cancel(x);
                    x = this.$timeout(() => {
                        scope.$emit("update-model");
                    }, 2500);
                });
        }


        static factory(): angular.IDirectiveFactory {
            const directive = ($timeout) => {
                return new InteractionDone($timeout);
            };
            directive["$inject"] = ["$timeout"];
            return directive;
        }
    }
    angular
        .module("app.flashcard")
        .directive("interactionDone", InteractionDone.factory());
}