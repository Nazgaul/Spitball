app.directive('productsMenu',
    ['$rootScope',

    function ($rootScope) {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                var $nav = $('nav'),
                    $body = $('body');

                elem.on('click', function (e) {
                    e.stopPropagation();
                    $nav.toggleClass('showProducts');
                });

                $rootScope.$on('$routeChangeStart', function () {
                    $nav.removeClass('showProducts');
                });

                $nav.on('mouseleave', function (e) {
                    
                    $nav.removeClass('showProducts');
                });

          ;
            }
        };
    }
    ]);
