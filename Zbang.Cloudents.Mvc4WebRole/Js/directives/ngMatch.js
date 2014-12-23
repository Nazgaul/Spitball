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
                    if (value) {
                        return scope.ngMatch.toLowerCase() === value.toLowerCase();
                    }
                    return false;
                };
                
                scope.$watch('ngMatch', function (newVal, oldVal) {
                    if (newVal !== oldVal) {
                        ngModel.$validate();
                    }
                });
            }
        }
    }]);