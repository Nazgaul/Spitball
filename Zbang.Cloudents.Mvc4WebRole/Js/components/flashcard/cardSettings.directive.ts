//module app {
//    "use strict";
//    class CardSettingsDirective implements angular.IDirective {
//        restrict = "A";

//        constructor(private $mdMedia) {
//        }
//        link = (scope: angular.IScope, element: JQuery, attrs: ng.IAttributes) => {
//            element.on("click",
//                () => {
//                    var selector = ".card-settings";
//                    var settings = element.closest("li").find(selector);
//                    //$(selector).not(settings).hide();
//                    $(selector).not(settings).removeClass("active");
//                    if (this.$mdMedia(attrs["cardSettings"])) {
//                        //settings.toggle();
//                        settings.toggleClass("active");
//                    }
//                });
//        }
//        static factory(): angular.IDirectiveFactory {
//            const directive = ($mdMedia) => {
//                return new CardSettingsDirective($mdMedia);
//            };

//            directive["$inject"] = ["$mdMedia"];

//            return directive;
//        }
//    }
//    angular
//        .module("app.flashcard")
//        .directive("cardSettings", CardSettingsDirective.factory());
//}

module app {
    "use strict";
    class LineFocus implements angular.IDirective {
        restrict = "A";

        constructor(private $timeout: angular.ITimeoutService) {
        }
        link = (scope: angular.IScope, element: JQuery) => {
            element.on("focusin click",
                (e) => {
                    var currentCheckBox = element.find(":checkbox");
                    $(":checked").not(currentCheckBox).prop("checked", false);
                    $(".focused").removeClass("focused");
                    element.addClass("focused");
                    $(e.target).closest("card-form").addClass("focused");
                    //element.closest("li").addClass("focused");
                })
                .on("focusout",
                () => {
                    // :focus need some timeout since the first element is document
                    this.$timeout(() => {
                        if ($(":focus").parents(".focused").length) {
                            return;
                        }
                        //$(":checked").prop("checked", false);
                        $(".focused").removeClass("focused");
                    });
                });
        }
        static factory(): angular.IDirectiveFactory {
            const directive = ($timeout) => {
                return new LineFocus($timeout);
            };
            directive["$inject"] = ["$timeout"];
            return directive;
        }
    }
    angular
        .module("app.flashcard")
        .directive("lineFocus", LineFocus.factory());
}