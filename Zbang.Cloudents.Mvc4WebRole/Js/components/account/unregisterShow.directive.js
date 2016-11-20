(function () {
    'use strict';
    angular.module('app').directive('unregisterShow', unregShow);
    unregShow.$inject = ['$window', '$rootScope'];

    function unregShow($window, $rootScope) {
        return {
            restrict: 'A',
            link: function (scope, element) {
                var css = 'smallHeight mediumHeight largeHeight noHeight';
                //var windowHeight = $(window).height();
                $rootScope.$on('show-unregisterd-box', function () {
                    element.removeClass(css).addClass('smallHeight');
                });
                $rootScope.$on('hide-unregisterd-box', function () {
                    element.removeClass(css).addClass('noHeight');
                });
                $rootScope.$on('show-unregisterd-box-medium', function () {
                    element.removeClass(css).addClass('mediumHeight');
                });
                //$rootScope.$on('show-unregisterd-box-large', function () {
                //    element.removeClass(css).addClass('largeHeight');
                //});

            }
        };
    }


})();
(function () {
    angular.module('app').directive('unregisterScroll', unregShow);
    unregShow.$inject = ['$window', '$rootScope', 'userDetailsFactory'];

    function unregShow($window, $rootScope, userDetailsFactory) {
        return {
            restrict: 'A',

            link: function (scope, element, attrs) {
                if (userDetailsFactory.isAuthenticated()) {
                    return;
                }
                var windowHeight = $(window).height();
                var $container = angular.element($window);

                $container.on('scroll', $handle);
                scope.$on('$destroy', function () {
                    $container.unbind('scroll', $handle);
                });

                function $handle() {
                    var scrollPos = $container.scrollTop();
                    //var unregContainer = $(".unreg-user .content-wrapper");
                    if (scrollPos > 500) {
                        $rootScope.$broadcast('show-unregisterd-box');
                        if (scrollPos > windowHeight * 0.5) {
                            $rootScope.$broadcast('show-unregisterd-box-medium');
                            //if (scrollPos > windowHeight * 0.75) {
                            //    $rootScope.$broadcast('show-unregisterd-box-large');
                            //}
                        }
                    } else {
                        $rootScope.$broadcast('hide-unregisterd-box');
                    }
                }
            }
        };
    }


})();

(function () {
    angular.module('app').directive('userNotRegisterClick', unregShow);
    unregShow.$inject = ['$window', '$rootScope', 'userDetailsFactory'];

    function unregShow($window, $rootScope, userDetailsFactory) {
        return {
            restrict: 'A',
            link: function (scope, element) {
                if (userDetailsFactory.isAuthenticated()) {
                    return;
                }
                element.on('click', 'a', function (e) {
                    e.preventDefault();
                    $rootScope.$broadcast('show-unregisterd-box');
                });


            }
        };
    }
})();
(function () {
    angular.module('app').directive('uiSrefAuth', unregShow);
    unregShow.$inject = ['$compile', 'userDetailsFactory', '$rootScope'];


    function unregShow($compile, userDetailsFactory, $rootScope) {
        return {
            scope: {
                val: '@uiSrefAuth'
                //if: '=uiSrefIf'
            },
            link: function ($scope, $element, $attrs) {
                $element.removeAttr('ui-sref-auth');
                $compile($element)($scope);

                // $scope.$watch('if', function (bool) {
                if (userDetailsFactory.isAuthenticated()) {
                    $element.attr('ui-sref', $scope.val);
                } else {
                    $element.removeAttr('ui-sref');
                    $element.removeAttr('href');
                    
                }
                $compile($element)($scope);
                //});
            }
        };
    }
})();

(function () {
    angular.module('app').directive('userNotRegisterPopup', unregShow);
    unregShow.$inject = ['$window', '$rootScope', 'userDetailsFactory', '$timeout'];

    function unregShow($window, $rootScope, userDetailsFactory, $timeout) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.on(attrs.userNotRegisterPopup, function (e) {
                    if (userDetailsFactory.isAuthenticated()) {
                        return;
                    }
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    element.blur();
                    //iphone takes time to close keyboard and then he kicks in the hide scroll - we put delay
                    $timeout(function () {
                        $rootScope.$broadcast('show-unregisterd-box');
                    }, 80);
                });
            }
        };
    }
})();