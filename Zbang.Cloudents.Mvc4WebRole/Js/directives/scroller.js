app.directive('scroller', function () {
    return {
        restrict: 'A',
        // new
        scope: {
            loadingMethod: "&"
        },
        link: function (scope, elem, attrs) {
            $(window).bind('scroll', function () {
                if ($(window).scrollTop() > $(document).height() - $(window).height() - 30) {
                    scope.$apply(scope.loadingMethod); //new
                }
            });
        }
    };
});