app.filter('fileSize',
[
    function () {

        function number_format(number, decimals, dec_point, thousands_sep) {
            // *     example 1: number_format(1234.5678, 2, '.', '');
            // *     returns 1: 1234.57     

            var n = number, c = isNaN(decimals = Math.abs(decimals)) ? 2 : decimals;
            var d = dec_point == undefined ? "," : dec_point;
            var t = thousands_sep == undefined ? "." : thousands_sep, s = n < 0 ? "-" : "";
            var i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "", j = (j = i.length) > 3 ? j % 3 : 0;

            return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
        }

        return function (filesize, parentecies) {
            if (filesize >= 1073741824) {
                filesize = number_format(filesize / 1073741824, 2, '.', '') + ' Gb';
            } else {
                if (filesize >= 1048576) {
                    filesize = number_format(filesize / 1048576, 2, '.', '') + ' Mb';
                } else {
                    if (filesize >= 1024) {
                        filesize = number_format(filesize / 1024, 0) + ' Kb';
                    } else {
                        filesize = number_format(filesize, 0) + ' bytes';
                    };
                };
            };
            return parentecies ? '(' + filesize + ')' : filesize;           
        };
    }
]);