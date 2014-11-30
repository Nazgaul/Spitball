app.directive('boxItemTooltip', ['$timeout', '$templateCache', '$compile',
function ($timeout, $templateCache, $compile) {
    "use strict";

    var tooltipTemplate,showTooltip = 'showTooltip';
    return {
        restrict: 'A',        
        link: function (scope, element, attrs) {
            var hoverIntentPromise,
                leaveIntentPromise,
                $body = angular.element(document.body),
                delay = 500,
                tooltipElement,
                tooltipTemplate = $templateCache.get(attrs.template);
           
            scope.$watch(attrs.boxItemTooltip, function (newValue) {
                scope[attrs.boxItemTooltip] = newValue || {};
            });

            element.on('mouseenter', function (event) {
                if (!tooltipElement) {
                    tooltipElement = angular.element(tooltipTemplate);
                    $body.append(tooltipElement)

                    //sUser.minProfile({ userId: attributes.userTooltipPopup }).then(function (response) {
                    //scope.user = response;
                    $compile(tooltipElement)(scope);
                    //});

                }

                hoverIntentPromise = $timeout(function () {
                    setPosition();
                    tooltipElement.addClass(showTooltip);

                    tooltipElement.on({
                        mouseenter: function () {
                            $timeout.cancel(leaveIntentPromise);
                        },
                        mouseleave: function () {
                            tooltipElement.removeClass(showTooltip);
                        }
                    });

                }, delay);


                function setPosition() {
                    var positionX, positionY, pos, offset,
                    isLtr = $('html').css('direction') === 'ltr';

                    pos = element[0].getBoundingClientRect();
                    positionY = pos.top - tooltipElement.outerHeight(true) - 5;

                    positionX = pos.left;
                    tooltipElement.css({ top: positionY, left: positionX });
                }
            });

            element.on('mouseleave', function () {
                $timeout.cancel(hoverIntentPromise);

                leaveIntentPromise = $timeout(function () {
                    tooltipElement.removeClass(showTooltip);
                }, 250);

            });

            scope.$on('$destroy', function () {
                if (tooltipElement) {
                    tooltipElement.remove();
                }
            });
        }
    };
}]);