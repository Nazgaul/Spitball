var app;
(function (app) {
    "use strict";
    var LineFocus = (function () {
        function LineFocus($timeout) {
            var _this = this;
            this.$timeout = $timeout;
            this.restrict = "A";
            this.link = function (scope, element) {
                element.on("focusin click", function (e) {
                    var currentCheckBox = element.find(":checkbox");
                    $(":checked").not(currentCheckBox).prop("checked", false);
                    $(".focused").removeClass("focused");
                    element.addClass("focused");
                    $(e.target).closest("card-form").addClass("focused");
                })
                    .on("focusout", function () {
                    _this.$timeout(function () {
                        if ($(":focus").parents(".focused").length) {
                            return;
                        }
                        $(".focused").removeClass("focused");
                    });
                });
            };
        }
        LineFocus.factory = function () {
            var directive = function ($timeout) {
                return new LineFocus($timeout);
            };
            directive["$inject"] = ["$timeout"];
            return directive;
        };
        return LineFocus;
    }());
    angular
        .module("app.flashcard")
        .directive("lineFocus", LineFocus.factory());
})(app || (app = {}));
