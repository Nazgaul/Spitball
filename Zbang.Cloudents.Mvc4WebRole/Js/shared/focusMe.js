(function () {

    angular.module('app').directive('focusMe', focusMe);
    focusMe.$inject = ['$timeout', '$parse'];
    function focusMe($timeout, $parse) {
        return {
            //scope: true,   // optionally create a child scope
            link: function (scope, element, attrs) {
                var model = $parse(attrs.focusMe);
                scope.$watch(model, function (value) {
                    if (value === true) {
                        $timeout(function () {
                            element[0].focus();
                        });
                    }
                });
                // to address @blesh's comment, set attribute value to 'false'
                // on blur event:
                //element.bind('blur', function () {
                //    scope.$apply(model.assign(scope, false));
                //});
            }
        };
    };
})();

//(function () {
//    angular.module('app').directive("contenteditable", function () {
//        return {
//            restrict: "A",
//            require: "ngModel",
//            link: function (scope, element, attrs, ngModel) {

//                function read() {
//                    ngModel.$setViewValue(element.html());
//                }

//                ngModel.$render = function () {
//                    element.html(ngModel.$viewValue || "");
//                };

//                element.bind("blur keyup change", function () {
//                    scope.$apply(read);
//                });
//            }
//        };
//    });
//})();

