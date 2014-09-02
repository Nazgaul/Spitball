app.directive('bagData',
    [
        function () {
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
    ]);
    