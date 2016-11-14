module app {
    "use strict";
    class FitText implements angular.IDirective {
        restrict = "A";
        link = (scope: angular.IScope, element: JQuery, attrs: ng.IAttributes) => {
            // var resizeText, _results1;
            var resizeTextSmall = () => {
                var elNewFontSize = (parseInt($(element).css('font-size').slice(0, -2)) - 1) + 'px';
                return $(element).css('font-size', elNewFontSize);
            };
            var resizeTextHeight = () => {
                var elNewFontSize = (parseInt($(element).css('font-size').slice(0, -2)) + 1) + 'px';
                return $(element).css('font-size', elNewFontSize);
            };

            scope.$watchGroup([attrs["ngBind"], "f.style"], (newValue) => {
                //$("[fit-text]").css('font-size', "");
                $(element).css('font-size', "");
                if (!newValue) {
                    return;
                }
                var resizeOccured: boolean;
                while (element[0].scrollHeight > element.parent()[0].offsetHeight && element[0].scrollWidth > element.parent()[0].offsetWidth) {
                    resizeTextSmall();
                    resizeOccured = true;
                }
                if (!resizeOccured) {
                    while (element[0].scrollHeight < element.parent()[0].offsetHeight && element[0].scrollWidth < element.parent()[0].offsetWidth) {
                        resizeTextHeight();
                    }
                }
            });

        }
        static factory(): angular.IDirectiveFactory {
            const directive = () => {
                return new FitText();
            };
            return directive;
        }
    }
    angular
        .module("app.flashcard")
        .directive("fitText", FitText.factory());
}