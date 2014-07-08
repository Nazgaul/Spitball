app.directive("ngPlaceholder",
    ['$log', '$timeout',

    function ($log, $timeout) {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                console.log(attrs);
                var txt = attrs.placeholder || attrs.ngPlaceholder,
                    model = attrs.ngModel,
                    placeholderSupport = Modernizr.input.placeholder;

                //Use HTML5 placeholder attribute.
                if (placeholderSupport) {
                    elem.attr('placeholder', elem.attr('ng-placeholder'));
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
