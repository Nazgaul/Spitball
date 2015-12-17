(function () {
    angular.module('app').directive('unregisterShow', unregShow);
    unregShow.$inject = ['$window', '$rootScope'];

    function unregShow($window, $rootScope) {
        return {
            restrict: 'A',
            link: function (scope, element) {

                $rootScope.$on('show-unregisterd-box', function () {
                    element.removeClass('noHeight').addClass('smallHeight');
                });

                $($window).scroll(function () {
                    var scrollPos = $(window).scrollTop();
                    //var unregContainer = $(".unreg-user .content-wrapper");
                    if (scrollPos > 0) {
                        element.removeClass('noHeight mediumHeight largeHeight').addClass('smallHeight');
                        if (scrollPos > 300) {
                            element.removeClass('smallHeight largeHeight').addClass('mediumHeight');
                            if (scrollPos > 600) {
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

    ;
})();