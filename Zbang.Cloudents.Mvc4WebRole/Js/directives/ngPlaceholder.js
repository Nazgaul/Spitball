app.directive("ngPlaceholder",
    ['$log', '$timeout',

    function ($log, $timeout) {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                var txt = attrs.ngPlaceholder,
                    model = attrs.ngModel,
                    placeholderSupport = Modernizr.input.placeholder;

                //Use HTML5 placeholder attribute.
                if (placeholderSupport) {
                    elem.attr('placeholder', txt);
                    elem.removeAttr('ng-placeholder');
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
        };
    }
    ]);
