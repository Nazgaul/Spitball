
app.filter('percentage', ['$window',
    function ($window) {
        "use strict";
        return function (input, decimals, suffix) {
            decimals = angular.isNumber(decimals) ? decimals : 3;
            suffix = suffix || '';
            if ($window.isNaN(input)) {
                return '0%';
            }
            return Math.round(input * Math.pow(10, decimals + 2)) / Math.pow(10, decimals) + '% ' + suffix
        };
    }]);