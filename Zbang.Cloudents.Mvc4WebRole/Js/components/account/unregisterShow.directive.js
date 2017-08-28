"use strict";
var app;
(function (app) {
    "use strict";
    var unregShow = (function () {
        function unregShow($rootScope) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.restrict = 'A';
            this.link = function (scope, element) {
                var css = 'smallHeight mediumHeight largeHeight noHeight';
                //var windowHeight = $(window).height();
                _this.$rootScope.$on("show-unregisterd-box", function () {
                    element.removeClass(css).addClass('smallHeight');
                });
                _this.$rootScope.$on("hide-unregisterd-box", function () {
                    element.removeClass(css).addClass('noHeight');
                });
                _this.$rootScope.$on("show-unregisterd-box-medium", function () {
                    element.removeClass(css).addClass('mediumHeight');
                });
                //$rootScope.$on('show-unregisterd-box-large', function () {
                //    element.removeClass(css).addClass('largeHeight');
                //});
            };
        }
        unregShow.factory = function () {
            var directive = function ($rootScope) {
                return new unregShow($rootScope);
            };
            directive["$inject"] = ["$rootScope"];
            return directive;
        };
        return unregShow;
    }());
    angular
        .module("app")
        .directive("unregisterShow", unregShow.factory());
})(app || (app = {}));
(function (app) {
    "use strict";
    var unregisterScroll = (function () {
        function unregisterScroll($window, $rootScope, userDetailsFactory) {
            var _this = this;
            this.$window = $window;
            this.$rootScope = $rootScope;
            this.userDetailsFactory = userDetailsFactory;
            this.restrict = 'A';
            this.link = function (scope, element, attrs) {
                var self = _this;
                if (_this.userDetailsFactory.isAuthenticated()) {
                    return;
                }
                var windowHeight = $(window).height();
                var $container = angular.element(_this.$window);
                $container.on('scroll', $handle);
                scope.$on('$destroy', function () {
                    $container.unbind('scroll', $handle);
                });
                function $handle() {
                    var scrollPos = $container.scrollTop();
                    //var unregContainer = $(".unreg-user .content-wrapper");
                    if (scrollPos > 500) {
                        self.$rootScope.$broadcast('show-unregisterd-box');
                        if (scrollPos > windowHeight * 0.5) {
                            self.$rootScope.$broadcast('show-unregisterd-box-medium');
                            //if (scrollPos > windowHeight * 0.75) {
                            //    $rootScope.$broadcast('show-unregisterd-box-large');
                            //}
                        }
                    }
                    else {
                        self.$rootScope.$broadcast('hide-unregisterd-box');
                    }
                }
            };
        }
        unregisterScroll.factory = function () {
            var directive = function ($window, $rootScope, userDetailsFactory) {
                return new unregisterScroll($window, $rootScope, userDetailsFactory);
            };
            directive["$inject"] = ['$window', '$rootScope', 'userDetailsFactory'];
            return directive;
        };
        return unregisterScroll;
    }());
    angular
        .module("app")
        .directive("unregisterScroll", unregisterScroll.factory());
})(app || (app = {}));
(function (app) {
    "use strict";
    var unregShow = (function () {
        function unregShow($window, $rootScope, userDetailsFactory) {
            var _this = this;
            this.$window = $window;
            this.$rootScope = $rootScope;
            this.userDetailsFactory = userDetailsFactory;
            this.restrict = 'A';
            this.link = function (scope, element) {
                if (_this.userDetailsFactory.isAuthenticated()) {
                    return;
                }
                if (element.is("a")) {
                    element.bind('click', 'a', function (e) {
                        e.preventDefault();
                        e.stopImmediatePropagation();
                        _this.$rootScope.$broadcast('show-unregisterd-box');
                    });
                }
                ;
                element.on('click', 'a', function (e) {
                    e.preventDefault();
                    _this.$rootScope.$broadcast('show-unregisterd-box');
                });
            };
        }
        unregShow.factory = function () {
            var directive = function ($window, $rootScope, userDetailsFactory) {
                return new unregShow($window, $rootScope, userDetailsFactory);
            };
            directive["$inject"] = ['$window', '$rootScope', 'userDetailsFactory'];
            return directive;
        };
        return unregShow;
    }());
    angular
        .module("app")
        .directive("userNotRegisterClick", unregShow.factory());
})(app || (app = {}));
(function (app) {
    "use strict";
    var unregShow = (function () {
        function unregShow($window, $rootScope, userDetailsFactory, $timeout) {
            var _this = this;
            this.$window = $window;
            this.$rootScope = $rootScope;
            this.userDetailsFactory = userDetailsFactory;
            this.$timeout = $timeout;
            this.restrict = 'A';
            this.link = function (scope, element, attrs) {
                element.on(attrs["userNotRegisterPopup"], function (e) {
                    if (_this.userDetailsFactory.isAuthenticated()) {
                        return;
                    }
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    element.blur();
                    //iphone takes time to close keyboard and then he kicks in the hide scroll - we put delay
                    _this.$timeout(function () {
                        _this.$rootScope.$broadcast('show-unregisterd-box');
                    }, 80);
                });
            };
        }
        unregShow.factory = function () {
            var directive = function ($window, $rootScope, userDetailsFactory, $timeout) {
                return new unregShow($window, $rootScope, userDetailsFactory, $timeout);
            };
            directive["$inject"] = ['$window', '$rootScope', 'userDetailsFactory', '$timeout'];
            return directive;
        };
        return unregShow;
    }());
    angular
        .module("app")
        .directive("userNotRegisterPopup", unregShow.factory());
})(app || (app = {}));
//####  OLD unregisterShow.js (no typescript):
//(function () {
//    'use strict';
//    angular.module('app').directive('unregisterShow', unregShow);
//    unregShow.$inject = ['$window', '$rootScope'];
//    function unregShow($window, $rootScope) {
//        return {
//            restrict: 'A',
//            link: function (scope, element) {
//                var css = 'smallHeight mediumHeight largeHeight noHeight';
//                //var windowHeight = $(window).height();
//                $rootScope.$on('show-unregisterd-box', function () {
//                    element.removeClass(css).addClass('smallHeight');
//                });
//                $rootScope.$on('hide-unregisterd-box', function () {
//                    element.removeClass(css).addClass('noHeight');
//                });
//                $rootScope.$on('show-unregisterd-box-medium', function () {
//                    element.removeClass(css).addClass('mediumHeight');
//                });
//                //$rootScope.$on('show-unregisterd-box-large', function () {
//                //    element.removeClass(css).addClass('largeHeight');
//                //});
//            }
//        };
//    }
//})();
//(function () {
//    angular.module('app').directive('unregisterScroll', unregShow);
//    unregShow.$inject = ['$window', '$rootScope', 'userDetailsFactory'];
//    function unregShow($window, $rootScope, userDetailsFactory) {
//        return {
//            restrict: 'A',
//            link: function (scope, element, attrs) {
//                if (userDetailsFactory.isAuthenticated()) {
//                    return;
//                }
//                var windowHeight = $(window).height();
//                var $container = angular.element($window);
//                $container.on('scroll', $handle);
//                scope.$on('$destroy', function () {
//                    $container.unbind('scroll', $handle);
//                });
//                function $handle() {
//                    var scrollPos = $container.scrollTop();
//                    //var unregContainer = $(".unreg-user .content-wrapper");
//                    if (scrollPos > 500) {
//                        $rootScope.$broadcast('show-unregisterd-box');
//                        if (scrollPos > windowHeight * 0.5) {
//                            $rootScope.$broadcast('show-unregisterd-box-medium');
//                            //if (scrollPos > windowHeight * 0.75) {
//                            //    $rootScope.$broadcast('show-unregisterd-box-large');
//                            //}
//                        }
//                    } else {
//                        $rootScope.$broadcast('hide-unregisterd-box');
//                    }
//                }
//            }
//        };
//    }
//})();
//(function () {
//    angular.module('app').directive('userNotRegisterClick', unregShow);
//    unregShow.$inject = ['$window', '$rootScope', 'userDetailsFactory'];
//    function unregShow($window, $rootScope, userDetailsFactory) {
//        return {
//            restrict: 'A',
//            link: function (scope, element) {
//                if (userDetailsFactory.isAuthenticated()) {
//                    return;
//                }
//                if (element.is("a")) {
//                    element.bind('click', 'a', function (e) {
//                        e.preventDefault();
//                        e.stopImmediatePropagation();
//                        $rootScope.$broadcast('show-unregisterd-box');
//                    });
//                };
//                element.on('click', 'a', function (e) {
//                    e.preventDefault();
//                    $rootScope.$broadcast('show-unregisterd-box');
//                });
//            }
//        };
//    }
//})();
//(function () {
//    angular.module('app').directive('userNotRegisterPopup', unregShow);
//    unregShow.$inject = ['$window', '$rootScope', 'userDetailsFactory', '$timeout'];
//    function unregShow($window, $rootScope, userDetailsFactory, $timeout) {
//        return {
//            restrict: 'A',
//            link: function (scope, element, attrs) {
//                element.on(attrs.userNotRegisterPopup, function (e) {
//                    if (userDetailsFactory.isAuthenticated()) {
//                        return;
//                    }
//                    e.preventDefault();
//                    e.stopImmediatePropagation();
//                    element.blur();
//                    //iphone takes time to close keyboard and then he kicks in the hide scroll - we put delay
//                    $timeout(function () {
//                        $rootScope.$broadcast('show-unregisterd-box');
//                    }, 80);
//                });
//            }
//        };
//    }
//})(); 
//# sourceMappingURL=unregisterShow.directive.js.map