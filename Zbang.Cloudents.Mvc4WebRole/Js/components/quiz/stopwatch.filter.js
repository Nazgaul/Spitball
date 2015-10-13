(function () {
    'use strict';

    angular.module('app.quiz.stopwatch').filter('stopwatch', [filter]);

    function filter() {
        var sK = 1000;
        var mK = 60 * sK;
        var hK = 60 * mK;

        return function (value) {
            //if (value == 0) {
            //    hours = minutes = seconds = 0;
            //    return addZero(hours) + ':' + addZero(minutes) + ':' + addZero(seconds);


            //}
            var hours = Math.floor(value / hK);
            value -= hours * hK;
            var minutes = Math.floor(value / mK);
            value -= minutes * mK;
            var seconds = Math.floor(value / sK);
                

            return addZero(hours) + ':' + addZero(minutes) + ':' + addZero(seconds);

        };

        function addZero(digit) {            
            return digit.toString().length < 2 ? '0' + digit : digit;
        }
    }
})();