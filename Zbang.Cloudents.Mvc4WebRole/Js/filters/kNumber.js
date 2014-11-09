app.filter('kNumber',
[
    function () {
        "use strict";
        return function (number) {
            
            if (!number) {
                return number;
            }

            return abbreviate();


            function abbreviate() {
                var abbr;
                if (number >= 1e12) {
                    abbr = 'T'
                }
                else if (number >= 1e9) {
                    abbr = 'B'
                }
                else if (number >= 1e6) {
                    abbr = 'M'
                }
                else if (number >= 1e3) {
                    abbr = 'K'
                }
                else {
                    abbr = ''
                }

                return annotate(abbr);
            }
            
            function annotate(abbr) {
                var rounded = 0
                switch (abbr) {
                    case 'T':
                        rounded = number / 1e12
                        break
                    case 'B':
                        rounded = number / 1e9
                        break
                    case 'M':
                        rounded = number / 1e6
                        break
                    case 'K':
                        rounded = number / 1e3
                        break
                    case '':
                        rounded = number
                        break
                }
                

                if (abbr) {
                    rounded = rounded.toFixed(2);
                }

                return rounded + abbr;
            }
        };
    }
]);
