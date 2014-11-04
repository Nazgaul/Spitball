
app.filter('fileSize',
[
    function () {
        "use strict";
        return function (filesize, parentecies) {

            var formatted = plupload.formatSize(filesize);

            return parentecies ? '(' + formatted + ')' : formatted;
        };
    }
]);