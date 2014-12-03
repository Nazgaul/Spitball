app.directive('boxItemTooltip', ['$timeout', '$templateCache', '$compile',
    function ($timeout, $templateCache, $compile) {
    "use strict";

    var tooltipTemplate, showTooltip = 'showTooltip';
    angular.element(window).on('scroll', function () {
        $('.showTooltip').removeClass(showTooltip);
    });
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {

            if (scope.$parent.$parent.getView() === 'itemListView') {
                return;
            }

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
                            tooltipElement.removeClass(showTooltip + ' top');
                        }
                    });

                }, delay);


                function setPosition() {
                    var positionX, positionY, pos, offset;
                    pos = element[0].getBoundingClientRect();
                    
                    offset = $(window).height() - (pos.bottom + tooltipElement.outerHeight(true));
                    if (offset < 0) {
                        positionY = pos.top - tooltipElement.height() - 15;
                        tooltipElement.addClass('top');
                    } else {
                        positionY = pos.bottom - 15;
                    }


                    positionX = pos.left + element.width() / 2 - tooltipElement.outerWidth(true) / 2;
                    tooltipElement.css({ top: positionY, left: positionX });
                }
            });

            element.on('mouseleave', function () {
                $timeout.cancel(hoverIntentPromise);

                leaveIntentPromise = $timeout(function () {
                    tooltipElement.removeClass(showTooltip + ' top');

                }, 250);

            });

            scope.$on('$destroy', destroy);
           
            function destroy() {
                if (tooltipElement) {
                    tooltipElement.remove();
                }
            }

        }
    };
}]);