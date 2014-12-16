app.directive('ngMatch',
    ['$parse', function ($parse) {
        return {
            require: 'ngModel',
            scope: {
                ngMatch : '='
            },
            link: function ($scope, element, attrs, ngModel) {
                ngModel.$validators.match = function (value) {
                    var toMatch = $parse(attrs.ngMatch);
                    return false;
                };
            }
        }
    }]);