app.directive('bagData',
    [
        function () {
        return {
            restrict: "A",
            scope: false,
            link: function (scope, elem, attrs) {

                scope.product = JSON.parse(elem.attr('bag-data'));
            }
        };
    }
    ]);
    