(function () {
    "use strict";
    app.directive('autoSelect', function () {
        return {
            restrict: 'A',
            link: function (scope, element) {
             
                select();

                var $copyText = angular.element('.fbCopyInfo');
                element.on('click', function () {
                    this.select();
                    $copyText.show();
                });

                element.on('blur', function () {
                    $copyText.hide();
                });

                scope.$on('input:facebook', select, 50);

                scope.$on('$destroy', function () {
                    elmene.off('click');
                    elmene.off('blur');                    
                });

                function select() {
                    setTimeout(function () {
                        element[0].focus();
                        element[0].select();
                    }, 50);

                }
            }
        };
    });
})();