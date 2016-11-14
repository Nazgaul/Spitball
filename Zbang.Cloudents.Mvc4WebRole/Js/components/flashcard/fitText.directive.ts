module app {
    "use strict";
    class FitText implements angular.IDirective {
        restrict = "A";
        link = (scope: angular.IScope, element: JQuery, attrs: ng.IAttributes) => {
           // var resizeText, _results1;
            var resizeText = () => {
                var elNewFontSize = (parseInt($(element).css('font-size').slice(0, -2)) - 1) + 'px';
                return $(element).css('font-size', elNewFontSize);
            };
            
            scope.$watch(attrs["ngBind"], () => {
                while (element[0].scrollHeight > element[0].offsetHeight) {
                    resizeText();
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