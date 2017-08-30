
app.filter('extToClass',
[
    function () {
        "use strict";
        return function (item) {
            if (!item) {
                return;
            }

            //if (item.type === 'Quiz') {
            //    return 'quiz';
            //}

            var prefix = 'mF';
            if (item.type === 'Link') {
                return prefix + 'link';
            }
            var cssClass = '',
                extension = item.name.slice(item.name.lastIndexOf('.') + 1);

            if (extension.length > 3) {
                cssClass += 'fourLetterExtension ';
            }
            return cssClass += prefix + extension.toLowerCase();
        };
    }
]);
