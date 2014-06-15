// Define you directives here. Directives can be added to same module as 'main' or a seperate module can be created.

angular.module('main.directives', []).
    directive("ngPlaceholder", function ($log, $timeout) {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                var txt = attrs.ngPlaceholder,
                    model = attrs.ngModel,
                    placeholderSupport = Modernizr.input.placeholder;

                //Use HTML5 placeholder attribute.
                if (placeholderSupport) {
                    return;
                }

                elem.removeAttr('placeholder');

                elem.on("focus", function (event) {
                    if (elem.val() === txt) {
                        elem.val("");
                    }
                });

                elem.on("blur", function (event) {
                    if (elem.val() === "") {
                        elem.val(txt);
                    }
                });

                scope.$watch(model, function (newValue, oldValue, scope) {
                    if (newValue === undefined || newValue === "") {
                        elem.val(txt);
                    }
                }, true);
            }
        }
    });