(function () {
    angular.module('app').directive('unregisterShow', unregShow);
    unregShow.$inject = ['$window'];

    function unregShow($window) {
        return {
            restrict: 'A',
            link: function (scope, element) {
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