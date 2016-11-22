(function() {
    'use strict';
    angular.module('app').directive('removeKeyboard', removeKeyboard);

    function removeKeyboard() {
        return {
            link: function(scope, element) {
                var textFields = $(element).children('input');

                $(element).submit(function(event) {
                    event.preventDefault();
                    textFields.blur();
                });
            }
        };
    }
})();
(function () {
    //TODO file
    'use strict';
    angular.module('app').directive('enterSubmit', enterSubmit);

    function enterSubmit() {
        return {
            link: function (scope, element, attrs) {
                //var textFields = $(element).children('input');

                element.bind('keydown', function (e) {
                    if (e.keyCode === 13) {
                        scope.$apply(function () {
                            scope.$eval(attrs.enterSubmit, { 'e': e });
                        });
                        //$(element).parents('form').submit();
                        e.preventDefault();
                    }
                });
                scope.$on("$destroy",
                    function() {
                        element.unbind('keydown');
                    });
            }
        };
    }
})();