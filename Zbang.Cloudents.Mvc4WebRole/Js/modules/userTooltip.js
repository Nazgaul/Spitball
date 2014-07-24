angular.module('mUserTooltip', []).
    directive('userTooltipPopup', ['$timeout', '$templateCache', '$compile', 'sUser',
        function ($timeout, $templateCache, $compile, sUser) {
            var tooltipTemplate = $templateCache.get('userToolTip.html');
            return {
                restrict: 'A',
                link: function (scope, element, attributes) {
                    var hoverIntentPromise,
                        $body = angular.element(document.body),
                        tooltipElement;

                    element.bind('mouseenter', function (event) {
                        tooltipElement = angular.element(tooltipTemplate);
                        $body.append(tooltipElement)

                        sUser.minProfile({ userId: attributes.userTooltipPopup }).then(function (response) {
                            scope.user = response.payload;
                            $compile(tooltipElement)(scope);
                        });

                        hoverIntentPromise = $timeout(function () {
                            tooltipElement.show();
                        }, delay);
                    });
                    element.bind('mouseleave', function () {
                        $timeout.cancel(hoverIntentPromise);
                        tooltipElement.remove();
                    });
                }
            };
        }]).
    factory('UserTooltipService',
    ['$templateCache', 'sUser',

    function ($templateCache, sUser) {


        return {
            loadUser: function (userId) {
                sUser.minProfile({ userId: userId }).then(function () {

                });
            }
        }
    }]);