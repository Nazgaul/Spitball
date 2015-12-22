(function () {
    angular.module('app').directive('unregisterShow', unregShow);
    unregShow.$inject = ['$window', '$rootScope'];

    function unregShow($window, $rootScope) {
        return {
            restrict: 'A',
            link: function (scope, element) {
                var windowHeight = $(window).height();
                $rootScope.$on('show-unregisterd-box', function () {
                    element.removeClass('noHeight').addClass('smallHeight');
                });

                $($window).scroll(function () {
                    var scrollPos = $(window).scrollTop();
                    //var unregContainer = $(".unreg-user .content-wrapper");
                    if (scrollPos > 0) {
                        element.removeClass('noHeight mediumHeight largeHeight').addClass('smallHeight');
                        if (scrollPos > windowHeight * 0.5) {
                            element.removeClass('smallHeight largeHeight').addClass('mediumHeight');
                            if (scrollPos > windowHeight * 0.75) {
                                element.removeClass('smallHeight mediumHeight').addClass('largeHeight');
                            }
                        }
                    }
                    else {
                        element.removeClass('smallHeight').addClass('noHeight');
                    }
                });
            }
        };
    }


})();//

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