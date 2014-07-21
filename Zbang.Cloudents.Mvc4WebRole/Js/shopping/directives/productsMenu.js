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

                $body.on('click', function (e) {
                    if ($(e.target).parents('nav').length) {
                        return;
                    }

                    if ($nav[0] === e.target) {
                        return;
                    }

                    $nav.removeClass('showProducts');
                });

          ;
            }
        };
    }
    ]);
