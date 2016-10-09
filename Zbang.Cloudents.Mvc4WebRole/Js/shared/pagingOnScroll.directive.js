
(function () {
    'use strict';
    angular.module('app').directive('ngScrollbarsPaging', ngScrollbarsPaging);
    var deepVal = function (path, obj) {
        if (path) {
            var ret = path.split('.')
                .reduce(function(prev, curr) {
                        return prev[curr];
                    },
                    obj || this);
            return ret;
        }
        return {};

    }

    //function doneScrolling(element, direction) {
    //    return ((direction == 'down' && element.offsetHeight + element.scrollTop == element.scrollHeight)
    //    || (direction == 'up' && element.scrollTop == 0));
    //}
    ngScrollbarsPaging.$inject = ['$timeout'];
    function ngScrollbarsPaging($timeout) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var callbacks, destroyed = false;
                switch (attrs.ngScrollbarsPaging) {
                    case 'up':
                        callbacks = {
                            onTotalScrollBack: function () {
                                var oldContentHeight = element.find('.mCSB_container').innerHeight();
                                runAction().then(function () {
                                    $timeout(function () {
                                        var heightDiff = element.find('.mCSB_container').innerHeight() - oldContentHeight;
                                        element.find('.content').mCustomScrollbar("scrollTo", "-=" + heightDiff, { scrollInertia: 0 });
                                    });
                                });
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
                deepVal(attrs.ngScrollbarsConfig, scope)['callbacks'] = callbacks;
                function runAction() {
                    if (destroyed) {
                        return;
                    }
                    return deepVal(attrs.ngScrollbarsPagingFunction, scope)();
                }
                scope.$on('$destroy', function () {
                    callbacks = null;
                    destroyed = true;
                });
            }
        };
    }
})();