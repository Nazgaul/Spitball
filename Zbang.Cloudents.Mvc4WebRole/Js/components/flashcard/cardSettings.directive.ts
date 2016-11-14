module app {
    class CardSettingsDirective implements angular.IDirective {
        restrict = "A";

        constructor(private $mdMedia) {
        }
        link = (scope: angular.IScope, element: JQuery, attrs: ng.IAttributes) => {
            element.on("click",
                () => {
                    var selector = ".card-settings";
                    var settings = element.closest("li").find(selector);
                    $(selector).not(settings).hide();
                    if (this.$mdMedia(attrs["cardSettings"])) {
                        settings.toggle();
                    }
                });
        }
        static factory(): angular.IDirectiveFactory {
            const directive = ($mdMedia) => {
                return new CardSettingsDirective($mdMedia);
            };

            directive['$inject'] = ['$mdMedia'];

            return directive;
        }
    }
    angular
        .module("app.flashcard")
        .directive("cardSettings", CardSettingsDirective.factory());
}