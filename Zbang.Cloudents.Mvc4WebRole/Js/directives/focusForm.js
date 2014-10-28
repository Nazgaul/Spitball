"use strict";
app.directive('focusForm',
    [
    function () {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                var x;
                elem.click(function (e) {
                    e.stopPropagation();
                    elem.addClass(attrs.focusForm);
                }).submit(function () {
                    elem.removeClass(attrs.focusForm);
                    x = null;
                }).on('reset', function () {
                    elem.removeClass(attrs.focusForm);
                    x = null;
                });
                var classes = '.uploader, .uploadLink';

                $('body').on('click', ':not(' + classes + ')', function (e) {
                    //.click(function () {
                        if (elem.hasClass(attrs.focusForm)) {
                            x = elem.removeClass(attrs.focusForm);
                        }
                    })
                .on('click', classes, function (e) {
                    e.stopPropagation();
                    if (x) {
                        x.addClass(attrs.focusForm);
                    }
                    x = null;
                });


            }
        };
    }
    ]);