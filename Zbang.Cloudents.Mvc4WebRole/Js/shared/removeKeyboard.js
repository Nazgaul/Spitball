(function() {
    angular.module('app').directive('removeKeyboard', removeKeyboard);

    function removeKeyboard() {
        return {
            link: function (scope, element) {
                var textFields = $(element).children('input');

                $(element).submit(function (event) {
                    event.preventDefault();
                    textFields.blur();
                });
            }
        };
    }
})()