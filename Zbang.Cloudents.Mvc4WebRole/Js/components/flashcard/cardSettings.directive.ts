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