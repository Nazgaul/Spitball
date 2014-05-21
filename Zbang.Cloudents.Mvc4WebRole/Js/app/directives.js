// Define you directives here. Directives can be added to same module as 'main' or a seperate module can be created.

var cloudentsDirectives = angular.module('main.directives', []);     //Define the directive module

cloudentsDirectives.directive('testDirective', function () {             //use as 'test-directive' in HTML
    return {
        link: function (scope, element, attrs) {
            console.log('Directive linked.');
        }
    };
});
cloudentsDirectives.directive('xngFocus', function () {
    return function(scope, element, attrs) {
        scope.$watch(attrs.xngFocus, 
          function (newValue) { 
              newValue && element.focus();
          },true);
    };    
});