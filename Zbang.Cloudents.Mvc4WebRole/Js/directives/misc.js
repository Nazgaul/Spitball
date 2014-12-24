app.directive('bagData',
    [
        function () {
            "use strict";

            return {
                restrict: "A",
                scope: false,
                link: function (scope, elem, attrs) {
                    var str = elem.attr('bag-data');
                    if (str) {
                        scope[attrs.bagDataProperty] = JSON.parse(str);
                    }
                }
            };
        }
    ]).
    directive('clearOnInput',
    [
        function () {
            "use strict";

            return {
                restrict: "A",
                scope: false,
                link: function (scope, elem, attrs) {
                    $(document).on('keyup', 'input', function () {
                        scope.$apply(function () {
                            scope.params.loginServerError = scope.params.registerServerError = null;
                        });
                    });

                    scope.$on('$destroy', function () {
                        $(document).off('keyup', 'input');
                    });
                }

                
            };
        }
    ]);

    