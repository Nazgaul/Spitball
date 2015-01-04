angular.module('app.events',[]).
    directive('cursorEnd',
        function () {
            return function (scope,element,attr) {
                element.on('focus', function () {
                    setTimeout(function () {
                        element[0].setSelectionRange(element[0].value.length, element[0].value.length);
                    }, 10);
                });

                scope.$on('destroy', function () {
                    element.off('focus');
                });
            }
        }
    
    
    )


