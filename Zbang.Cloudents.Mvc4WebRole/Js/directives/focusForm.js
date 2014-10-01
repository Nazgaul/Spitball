﻿app.directive('focusForm',
    [

    function () {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                elem.click(function(e) {
                    e.stopPropagation();
                    elem.addClass(attrs.focusForm);
                }).submit(function() {
                    elem.removeClass(attrs.focusForm);
                }).on('reset',function() {
                    elem.removeClass(attrs.focusForm);
                });
                $('body').on('click', '[role="dialog"]', function(e) {
                    e.stopPropagation();
                    elem.addClass(attrs.focusForm);
                })
                .click(function () {
                    elem.removeClass(attrs.focusForm);
                });


            }
        };
    }
    ]);