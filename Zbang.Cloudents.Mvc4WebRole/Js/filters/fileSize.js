"use strict";
app.filter('fileSize',
[
    function () {

        return function (filesize, parentecies) {

            var formatted = plupload.formatSize(filesize);

            return parentecies ? '(' + formatted + ')' : formatted;
        };
    }
]);