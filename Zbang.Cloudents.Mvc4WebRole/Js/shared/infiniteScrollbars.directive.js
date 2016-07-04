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
                var callbacks, destroyed = false;
                switch (attrs['infiniteScrollbarsDirection']) {
                    case 'up':
                        callbacks = {
                            onTotalScrollBack: function () {
                                var oldContentHeight = element.find('.mCSB_container').innerHeight();
                                runAction();
                                setTimeout(function () {
                                    var heightDiff = element.find('.mCSB_container').innerHeight() - oldContentHeight;
                                    element.find('.content').mCustomScrollbar("scrollTo", "-=" + heightDiff, { scrollInertia: 0 });
                                }, 30);
                            },

                            onTotalScrollOffset: 100

                        };
                        break;
                    default://down
                        callbacks = {
                            onTotalScroll: function () {
                                runAction();
                            }
                        };
                }

                deepVal(attrs['ngScrollbarsConfig'], scope)['callbacks'] = callbacks;

                function runAction() {
                    if (destroyed) {
                        return
                    }
                    deepVal(attrs['infiniteScrollbarsFunction'], scope)();
                }


                scope.$on('$destroy', function () {
                    callbacks = null;
                    destroyed = true;
                });
            }
        };
    }
})();