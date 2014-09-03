app.directive('productsMenu',
    ['$rootScope',

    function ($rootScope) {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                var $nav = $('nav');

                elem.on('click', function (e) {
                    e.stopPropagation();
                    $nav.toggleClass('showProducts');
                });

                //TODO: we trigger this events twice because this directive happen twice
                $rootScope.$on('$routeChangeStart', function () {
                    $nav.removeClass('showProducts');
                });

                $nav.on('mouseleave', function (e) {
                    $nav.removeClass('showProducts');
                });
                $(window).scroll(function () {
                    $nav.removeClass('showProducts');
                });


            }
        };
    }
    ]);
