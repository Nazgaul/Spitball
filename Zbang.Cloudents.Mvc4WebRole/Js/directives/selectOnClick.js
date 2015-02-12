(function () {
    "use strict";
    app.directive('selectOnClick', function () {        
        return {
            restrict: 'A',
            link: function (scope, element) {
                element.on('click', function () {
                    this.select();
                });
            }
        };
    }).directive('autoSelect', function () {     
        return {
            restrict: 'A',
            link: function (scope, element) {                
                scope.$on('search:select', function () {
                    element[0].select();
                });
            }
        };
    });
})();