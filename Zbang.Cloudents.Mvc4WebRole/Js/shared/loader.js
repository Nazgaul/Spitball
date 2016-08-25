'use strict';
(function () {
    angular.module('app').directive('ngSpinnerBar', [
        '$rootScope', '$timeout',
        function ($rootScope, $timeout) {
            return {
                //TODO: write this better
                link: function (scope, element, attrs) {
                    // by defult hide the spinner bar
                    element.addClass('hide'); // hide spinner bar by default
                    // display the spinner bar whenever the route changes(the content part started loading)
                    $rootScope.$on('$stateChangeStart', function () {
                        element.removeClass('hide'); // show spinner bar  
                    });

                    // hide the spinner bar on rounte change success(after the content loaded)
                    $rootScope.$on('$stateChangeSuccess', function () {
                        element.addClass('hide'); // hide spinner bar
                        //$('body').removeClass('page-on-load');//.scrollTop(); // remove page loading indicator
                    });

                    // handle errors
                    $rootScope.$on('$stateNotFound', function () {
                        element.addClass('hide'); // hide spinner bar
                    });

                    // handle errors
                    $rootScope.$on('$stateChangeError', function () {
                        element.addClass('hide'); // hide spinner bar
                    });

                    $rootScope.$on('state-change-start-prevent', function () {
                        $timeout(function () {
                            element.addClass('hide'); // hide spinner bar
                        }, 1);
                    });
                }
            };
        }
    ]);
})();

(function () {
    angular.module('app').directive('cursorLoader', [
        function () {
            return {
                scope: {
                    data: "=ngDisabled"
                },
                link: function (scope, element, attrs) {
                    var $pageBody = $('.page-body');

                    scope.$watch(function () {
                        return scope.data
                    }, function (loading) {
                        if (loading) {
                            $pageBody.addClass('loading-cursor');
                        }
                        else {
                            $pageBody.removeClass('loading-cursor');
                        }
                    });

                    scope.$on('$destroy', function () {
                        $pageBody.removeClass('loading-cursor');
                    });
                }
            };
        }
    ]);
})();