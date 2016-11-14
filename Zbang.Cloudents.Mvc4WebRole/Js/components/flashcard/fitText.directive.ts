module app {
    "use strict";
    class FitText implements angular.IDirective {
        restrict = "A";
        link = (scope: angular.IScope, element: JQuery, attrs: ng.IAttributes) => {
           // var resizeText, _results1;
            //var resizeTextSmall = () => {
            //    var elNewFontSize = (parseInt($(element).css('font-size').slice(0, -2)) - 1) + 'px';
            //    return $(element).css('font-size', elNewFontSize);
            //};
            //var resizeTextHeight = () => {
            //    var elNewFontSize = (parseInt($(element).css('font-size').slice(0, -2)) + 1) + 'px';
            //    return $(element).css('font-size', elNewFontSize);
            //};
            var attributeToChange = "font-size";
            var changeFontSize = (isPositive) => {
                var change = isPositive ? 1 : -1;
                var elNewFontSize = (parseInt($(element).css(attributeToChange).slice(0, -2)) + change) + 'px';
                return $(element).css(attributeToChange, elNewFontSize);
            }

            scope.$watchGroup([attrs["ngBind"], "f.style"], (newValue) => {
                $(element).css(attributeToChange, "");
                if (!newValue[0]) {
                    return;
                }
                if (element[0].scrollHeight > element.parent()[0].offsetHeight) {
                    while (element[0].scrollHeight > element.parent()[0].offsetHeight) {
                        changeFontSize(false);
                        
                    }
                } else {
                    while (element[0].scrollHeight < element.parent()[0].offsetHeight) {
                        changeFontSize(true);
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