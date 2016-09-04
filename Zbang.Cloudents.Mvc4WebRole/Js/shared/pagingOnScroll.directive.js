(function () {
    'use strict';
    angular.module('app').directive('ngScrollbarsPaging', ngScrollbarsPaging);
    var deepVal = function (path, obj) {
        var ret = path.split('.').reduce(function (prev, curr) {
            return prev[curr];
        }, obj || this);
        return ret;
    }

    //function doneScrolling(element, direction) {
    //    return ((direction === 'down'
    //        && element.offsetHeight + element.scrollTop === element.scrollHeight)
    //    || (direction === 'up' && element.scrollTop === 0));
    //}
    function doneScrolling(element, direction) {
        if (direction === "down") {
            $(window).scrollTop() + $(window).innerHeight() >= $('body')[0].scrollHeight
        }
    }

    ngScrollbarsPaging.$inject = ['$timeout', "$window"];
    function ngScrollbarsPaging($timeout, $window) {
        return {

            restrict: 'A',
            link: function (scope, element, attrs) {
                var callbacks, destroyed = false;
                if (mobileScroll()) {
                    var scrollElement = angular.element($window);
                    if (attrs["container"]) {
                        scrollElement = element;
                    }
                    scrollElement.bind('scroll', function () {
                        if (doneScrolling(scrollElement[0], attrs.direction)) {
                            switch (attrs.direction) {
                                case 'up':
                                    var oldContentHeight = scrollElement[0].scrollHeight;
                                    runAction().then(function () {
                                        $timeout(function () {
                                            var heightDiff = scrollElement[0].scrollHeight - oldContentHeight;
                                            scrollElement.scrollTop(heightDiff);
                                        });
                                    });
                                    break;
                                default: //down
                                    runAction();
                            }
                        }
                    });
                }

                else {
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
                }

                function runAction() {
                    if (destroyed) {
                        return;
                    }
                    return deepVal(attrs.ngScrollbarsPagingFunction, scope)();
                }


                function mobileScroll() {
                    return Modernizr.touchevents && attrs.disableTouch !== undefined;
                }

                scope.$on('$destroy', function () {
                    callbacks = null;
                    destroyed = true;
                    var scrollElement = angular.element("body");
                    if (attrs["container"]) {
                        scrollElement = element;
                    }
                    scrollElement.unbind('scroll');
                });


            }
        };
    }
})();


(function () {
    "use strict";
    angular.module('app').directive('scrollChooser', scrollChooser);
    scrollChooser.$inject = ["$compile"];

    function scrollChooser($compile) {
        return {
            restrict: 'A',
            replace: false,
            terminal: true,
            priority: 1000,
            link: function link(scope, element, attrs) {
                if (mobileScroll()) {
                    element
                        .removeAttr("disable-touch ng-scrollbars ng-scrollbars-paging ng-scrollbars-paging-function ng-scrollbars-config");
                    //disable-touch ng-scrollbars ng-scrollbars-paging="down" ng-scrollbars-paging-function="c.usersPaging" ng-scrollbars-config="c.scrollSetting"
                    //element.
                } else {
                    element
                       .removeAttr("srph-infinite-scroll");
                }
                element.removeAttr("scroll-chooser"); //remove the attribute to avoid indefinite loop
//also remove the same attribute with data- prefix in case users specify data-common-things in the html

                $compile(element)(scope);
                function mobileScroll() {
                    return Modernizr.touchevents && attrs.disableTouch !== undefined;
                }
            }

        }

        
    }
})();