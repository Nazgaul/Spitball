app.directive('focusForm',
    [

    function () {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                elem.click(function (e) {

                    e.stopPropagation();
                    elem.addClass(attrs.focusForm);
                }).submit(function () {
                    elem.removeClass(attrs.focusForm);
                });

                $('body').click(function () {
                    elem.removeClass(attrs.focusForm);
                });

            }
        };
    }
    ]);