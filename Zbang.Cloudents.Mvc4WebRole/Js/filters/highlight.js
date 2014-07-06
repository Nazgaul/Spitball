
define('highlight', ['app'], function (app) {
    app.filter('highlight',
    [
        function () {
            return function (text, search, caseSensitive, className) {
                className = className || 'boldPart';

                if (text && (search || angular.isNumber(search))) {
                    text = text.toString();
                    search = search.toString();
                    if (caseSensitive) {
                        return text.split(search).join('<mark class="' + className + '">' + search + '</mark>');
                    } else {
                        return text.replace(new RegExp(search, 'gi'), '<mark class="' + className + '">$&</mark>');
                    }
                } else {
                    return text;
                }
            };
        }
    ]);       
});