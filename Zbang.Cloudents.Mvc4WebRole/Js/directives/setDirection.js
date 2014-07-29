app.directive('setDirection', ['$timeout', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            scope.$watch(attrs.ngModel, function (v) {
                $(element[0]).css(getDirection(v));
            });
            
            function getDirection(value) {
                var rtlChars = '\u0600-\u06FF' + '\u0750-\u077F' + '\u08A0-\u08FF' + '\uFB50-\uFDFF' + '\uFE70-\uFEFF';//arabic
                rtlChars += '\u0590-\u05FF' + '\uFB1D-\uFB4F';//hebrew

                var controlChars = '\u0000-\u0020';
                controlChars += '\u2000-\u200D';

                //Start Regular Expression magic
                var reRtl = new RegExp('[' + rtlChars + ']', 'g'),
                    reNotRtl = new RegExp('[^' + rtlChars + controlChars + ']', 'g'),
                    textAlign = $('html').css('direction') === 'ltr' ? 'left' : 'right';

               
                if (!value) {
                    value = attrs.ngPlaceholder;
                }

                return checkRtlDirection(value) ?
                        { direction: 'rtl', textAlign: 'right' } : { direction: 'ltr', textAlign: 'left' }


                function checkRtlDirection(value) {                  

                    var rtls = value.match(reRtl);
                    if (rtls !== null)
                        rtls = rtls.length;
                    else
                        rtls = 0;

                    var notrtls = value.match(reNotRtl);
                    if (notrtls !== null)
                        notrtls = notrtls.length;
                    else
                        notrtls = 0;

                    return rtls > notrtls;
                }
            }
        }
    };
}]);