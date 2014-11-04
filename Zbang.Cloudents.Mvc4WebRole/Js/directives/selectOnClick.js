﻿
app.directive('selectOnClick', function () {
    "use strict";
    return {
        restrict: 'A',
        link: function (scope, element) {
            element.on('click', function () {
                this.select();
            });
        }
    };
});
