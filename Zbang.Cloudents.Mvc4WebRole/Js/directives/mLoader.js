"use strict";
app.directive('loader',
    ['$rootScope', '$timeout',

    function ($rootScope, $timeout) {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {

                $rootScope.$on('$routeChangeStart', function () {
                    elem.css({ opacity: 1, display: 'block' });
                    var $view = $(document.querySelector('.page'));
                    $view.css({ opacity: 0, display: 'none' });
                });


                $rootScope.$on('viewContentLoaded', function () {
                    elem.css({ opacity: 0, display: 'none' });
                    var $view = $(document.querySelector('.page'));
                    $view.css({ opacity: 1, display: 'block' });
                    cd.pubsub.publish('showTime');

                });
            }
        };
    }
    ]);
