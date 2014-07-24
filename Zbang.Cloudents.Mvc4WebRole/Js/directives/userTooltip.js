angular.module('UserTooltip', []).
    directive('userTooltip', ['$timeout', 'sUser', function ($timeout, sUser) {
        return {
            restrict: 'A',
            link: function (scope, element, attributes) {
                var hoverIntentPromise;
                element.bind('mouseenter', function (event) {

                    hoverIntentPromise = $timeout(function () {
                        scope.$eval(attributes.hoverIntent, { $event: event });
                    }, delay);
                });
                element.bind('mouseleave', function () {
                    $timeout.cancel(hoverIntentPromise);
                });
            }
        };
    }]).
    factory()