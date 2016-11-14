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
            
            scope.$watch(attrs["ngBind"], (newValue) => {
                $(element).css('font-size', "");
                if (!newValue) {
                    return;
                }
                if (element[0].scrollHeight > element.parent()[0].offsetHeight) {
                    while (element[0].scrollHeight > element.parent()[0].offsetHeight) {
                        resizeTextSmall();
                    }
                } else {
                    while (element[0].scrollHeight < element.parent()[0].offsetHeight) {
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