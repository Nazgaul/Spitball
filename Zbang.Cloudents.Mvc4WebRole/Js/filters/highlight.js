app.filter('highlight',
[
    function () {
        "use strict";
        return function (text, search, caseSensitive, className) {

            function mark(word,string) {
                if (caseSensitive) {
                    return string.split(word).join('<mark class="' + className + '">' + search + '</mark>');
                } else {
                    return string.replace(new RegExp(word, 'gi'), '<mark class="' + className + '">$&</mark>');
                }
            }

            className = className || 'boldPart';

            if (text && (search || angular.isNumber(search))) {
                text = text.toString();
                search = search.toString();

                if (search.indexOf(' ') === -1) {
                    return mark(search,text);
                }

                var array = search.split(' '),
                    result = text;

                _.forEach(array, function (key) {
                    result = mark(key,result);
                });

                return result;

                
                
            } else {
                return text;
            }
        };
    }
]);
