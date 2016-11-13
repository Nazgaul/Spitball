module app {
    class CardSettingsDirective implements angular.IDirective {
        restrict = "A";

        constructor(private $mdMedia) {
        }
        link = (scope: angular.IScope, element: JQuery, attrs: ng.IAttributes) => {

            var allCardsSettings = $(".card-settings");
            var currentCardSettings = $(attrs["cardSettings"]);
            element.on(attrs["cardSettingsEvent"], () => {
                if (!this.$mdMedia('xs')) {
                    allCardsSettings.hide();
                    currentCardSettings.show();
                }
            });

        }
        public static factory(): angular.IDirectiveFactory {
            var directive = ($mdMedia) => {
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