//scrollbarsInfiniteScroll
'use strict';

(function () {
    angular.module('app').directive('infiniteScrollbarsDirection', infiniteScrollbarsDirection);
    function infiniteScrollbarsDirection() {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {

            }
        };
    }

})();

(function () {
    angular.module('app').directive('infiniteScrollbarsFunction', infiniteScrollbarsFunction);

    var deepVal = function (path, obj) {
        var ret = path.split('.').reduce(function (prev, curr) {
            return prev[curr];
        }, obj || this);
        return ret;
    }
    function infiniteScrollbarsFunction() {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var callbacks;
                switch (attrs['infiniteScrollbarsDirection']) {
                    case 'up':
                        callbacks = {
                            onTotalScrollBack: function () {
                                console.log('scrolled up');
                                runAction();
                            }
                        };
                        break;
                    default://down
                        callbacks = {
                            onTotalScroll: function () {
                                console.log('scrolled down');
                                runAction();
                            }
                        };
                }

                deepVal(attrs['ngScrollbarsConfig'], scope)['callbacks'] = callbacks;

                function runAction() {
                    deepVal(attrs['infiniteScrollbarsFunction'], scope)();
                }
            }
        };
    }
})();