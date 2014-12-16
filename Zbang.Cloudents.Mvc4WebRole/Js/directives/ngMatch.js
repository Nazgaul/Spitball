app.directive('ngMatch',
    [function () {
        return {
            require: 'ngModel',
            scope: {
                ngMatch: '='
            },
            link: function (scope, element, attrs, ngModel) {
                ngModel.$validators.match = function (value) {
                    if (_.isUndefined(scope.ngMatch)) {
                        return true;
                    }
                    return scope.ngMatch.toLowerCase() === value.toLowerCase();
                };
                
                scope.$watch('ngMatch', function (newVal, oldVal) {
                    if (newVal !== oldVal) {
                        ngModel.$validate();
                    }
                });
            }
        }
    }]);