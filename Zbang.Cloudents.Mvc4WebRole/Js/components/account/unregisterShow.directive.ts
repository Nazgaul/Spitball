module app {
    "use strict";
    class unregShow implements angular.IDirective {
        restrict = 'A';

        constructor(private $rootScope: angular.IRootScopeService) {
        }

        link = (scope, element: JQuery) => {
            var css = 'smallHeight mediumHeight largeHeight noHeight';
            //var windowHeight = $(window).height();
            this.$rootScope.$on("show-unregisterd-box",
                () => {
                    element.removeClass(css).addClass('smallHeight');
                });
            this.$rootScope.$on("hide-unregisterd-box",
                () => {
                    element.removeClass(css).addClass('noHeight');
                });
            this.$rootScope.$on("show-unregisterd-box-medium",
                () => {
                    element.removeClass(css).addClass('mediumHeight');
                });
            //$rootScope.$on('show-unregisterd-box-large', function () {
            //    element.removeClass(css).addClass('largeHeight');
            //});
        }

        static factory(): angular.IDirectiveFactory {
            const directive = ($rootScope) => {
                return new unregShow($rootScope);
            };
            directive["$inject"] = ["$rootScope"];
            return directive;
        }
    }
    angular
        .module("app")
        .directive("unregisterShow", unregShow.factory());
}


module app {
    "use strict";
    class unregisterScroll implements angular.IDirective {
        restrict = 'A';

        constructor(private $window: angular.IWindowService, 
            private $rootScope: angular.IRootScopeService,
            private userDetailsFactory: IUserDetailsFactory) {
        }

        link = (scope, element: JQuery, attrs: ng.IAttributes) => {
            var self = this;
            if (this.userDetailsFactory.isAuthenticated()) {
                return;
            }
            var windowHeight = $(window).height();
            var $container = angular.element(this.$window);

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
                } else {
                    self.$rootScope.$broadcast('hide-unregisterd-box');
                }
            }
        }

        static factory(): angular.IDirectiveFactory {
            const directive = ($window, $rootScope, userDetailsFactory) => {
                return new unregisterScroll($window, $rootScope, userDetailsFactory);
            };
            directive["$inject"] = ['$window', '$rootScope', 'userDetailsFactory'];
            return directive;
        }
    }
    angular
        .module("app")
        .directive("unregisterScroll", unregisterScroll.factory());
}

module app {
    "use strict";
    class unregShow implements angular.IDirective {
        restrict = 'A';

        constructor(private $window: angular.IWindowService,
            private $rootScope: angular.IRootScopeService,
            private userDetailsFactory: IUserDetailsFactory) {
        }

        link = (scope, element: JQuery) => {
            if (this.userDetailsFactory.isAuthenticated()) {
                return;
            }
            if (element.is("a")) {
                element.bind('click', 'a', (e) => {
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    this.$rootScope.$broadcast('show-unregisterd-box');
                });
            };
            element.on('click', 'a', (e) => {
                e.preventDefault();
                this.$rootScope.$broadcast('show-unregisterd-box');
            });
        }

        static factory(): angular.IDirectiveFactory {
            const directive = ($window, $rootScope, userDetailsFactory) => {
                return new unregShow($window, $rootScope, userDetailsFactory);
            };
            directive["$inject"] = ['$window', '$rootScope', 'userDetailsFactory'];
            return directive;
        }
    }
    angular
        .module("app")
        .directive("userNotRegisterClick", unregShow.factory());
}


module app {
    "use strict";
    class unregShow implements angular.IDirective {
        restrict = 'A';

        constructor(private $window: angular.IWindowService,
            private $rootScope: angular.IRootScopeService,
            private userDetailsFactory: IUserDetailsFactory,
            private $timeout: angular.ITimeoutService) {
        }

        link = (scope, element: JQuery, attrs: ng.IAttributes) => {
            element.on(attrs["userNotRegisterPopup"], (e) => {
                if (this.userDetailsFactory.isAuthenticated()) {
                    return;
                }
                e.preventDefault();
                e.stopImmediatePropagation();
                element.blur();
                //iphone takes time to close keyboard and then he kicks in the hide scroll - we put delay
                this.$timeout(() => {
                    this.$rootScope.$broadcast('show-unregisterd-box');
                }, 80);
            });
        }

        static factory(): angular.IDirectiveFactory {
            const directive = ($window, $rootScope, userDetailsFactory, $timeout) => {
                return new unregShow($window, $rootScope, userDetailsFactory, $timeout);
            };
            directive["$inject"] = ['$window', '$rootScope', 'userDetailsFactory', '$timeout'];
            return directive;
        }
    }
    angular
        .module("app")
        .directive("userNotRegisterPopup", unregShow.factory());
}




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