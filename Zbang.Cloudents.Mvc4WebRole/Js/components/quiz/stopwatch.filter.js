(function () {
    'use strict';

    angular.module('app.quiz.stopwatch').filter('stopwatch', [filter]);

    function filter() {
        return function (value) {
            var seconds = Math.round(value / 1000),
                minutes = Math.round(seconds / 60),
                hours = Math.round(minutes / 60);

            return hours + ':' + minutes + ':' + seconds;

        };
    }
})();