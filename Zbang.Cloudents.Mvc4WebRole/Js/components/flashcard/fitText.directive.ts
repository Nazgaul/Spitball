module app {
    "use strict";
    class FitText implements angular.IDirective {

        constructor(private $animate: angular.animate.IAnimateService) {

        }
        restrict = "A";
        link = (scope: angular.IScope, element: JQuery, attrs: ng.IAttributes) => {
            var attributeToChange = "font-size";
            var changeFontSize = (isPositive) => {
                var change = isPositive ? 1 : -1, currentFontSize = parseInt($(element).css(attributeToChange).slice(0, -2));
                if (currentFontSize <= 18) {
                    return false;
                }
                var elNewFontSize = (currentFontSize + change) + 'px';

                $(element).css(attributeToChange, elNewFontSize);
                return true;
            }


            scope.$watchGroup([attrs["ngBind"], "f.style", "f.slidepos"], (newValue) => {
                this.$animate.removeClass(element.parents("angular-animate"), "ng-hide")
                    .then(() => {
                        changeFont();
                    });
                this.$animate.removeClass(element.parents("angular-animate"), "front back both")
                    .then(() => {
                        changeFont();
                    });
                changeFont();
                function changeFont() {
                    $(element).css(attributeToChange, "").removeClass("lessText");
                    if (!newValue[0]) {
                        return;
                    }

                    if (element.parents(".ng-hide").length) {
                        return;
                    }
                    if (newValue[0].length <= 80) {
                        element.addClass("lessText");
                    }
                    //var resizeOccured = false;
                    while (element[0].scrollHeight > element.parent()[0].offsetHeight ||
                        element[0].scrollWidth > element.parent()[0].offsetWidth) {
                        if (!changeFontSize(false)) {
                            break;
                        }
                        //resizeOccured = true;
                    }
                    //if (!resizeOccured) {
                    //    while (element[0].scrollHeight < element.parent().height() &&
                    //        element[0].scrollWidth < element.parent().width()) {
                    //        changeFontSize(true);
                    //    }
                    //}
                }
            });

        }
        static factory(): angular.IDirectiveFactory {
            const directive = ($animate) => {
                return new FitText($animate);
            };

            directive["$inject"] = ["$animate"];
            return directive;
        }
    }
    angular
        .module("app.flashcard")
        .directive("fitText", FitText.factory());
}

module app {
    "use strict";
    class CardSlideAnimation implements angular.IDirective {

        constructor(private $animate: angular.animate.IAnimateService) {

        }
        scope: { [boundProperty: string]: string } = {
            'myHide': '='
            //'afterShow': '&',
            //'afterHide': '&'
        }
        restrict = "A";
        link = (scope: angular.IScope, element: JQuery, attrs: ng.IAttributes) => {
            //this.$animate.
            scope.$watch('myHide', (show, oldShow) => {
                if (!show) {
                    $("body").css("overflow", "hidden");
                    this.$animate.removeClass(element, "ng-hide")
                        .then(() => {
                            $("body").css("overflow", "");
                        });
                }
                else {
                    $("body").css("overflow", "hidden");
                    this.$animate.addClass(element, "ng-hide").then(() => {
                        $("body").css("overflow", "");
                    });
                }
            });
        }
        static factory(): angular.IDirectiveFactory {
            const directive = ($animate) => {
                return new CardSlideAnimation($animate);
            };

            directive["$inject"] = ["$animate"];
            return directive;
        }
    }
    angular
        .module("app.flashcard")
        .directive("myHide", CardSlideAnimation.factory());
}