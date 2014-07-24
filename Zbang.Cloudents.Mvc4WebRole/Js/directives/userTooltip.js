app.directive('userTooltipPopup', ['$timeout', '$templateCache', '$compile', 'sUser', 'sUserDetails',
function ($timeout, $templateCache, $compile, sUser, sUserDetails) {
    var tooltipTemplate = $templateCache.get('userToolTip.html');
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {
            var hoverIntentPromise,
                leaveIntentPromise,
                $body = angular.element(document.body),
                delay = 500,
                userId = parseInt(attributes.userTooltipPopup, 10),
                tooltipElement;

            if (!userId) {
                return;
            }
            if (userId === sUserDetails.getDetails().id) {
                return;
            }

            element.on('mouseenter', function (event) {
                if (!tooltipElement) {
                    tooltipElement = angular.element(tooltipTemplate);
                    $body.append(tooltipElement)

                    sUser.minProfile({ userId: attributes.userTooltipPopup }).then(function (response) {
                        scope.user = response.payload;
                        $compile(tooltipElement)(scope);
                    });

                }

                hoverIntentPromise = $timeout(function () {
                    setPosition();
                    tooltipElement.addClass('showTooltip');

                    tooltipElement.on({
                        mouseenter: function () {
                            $timeout.cancel(leaveIntentPromise);
                        },
                        mouseleave: function () {
                            tooltipElement.removeClass('showTooltip');
                        },
                        click: function () {
                            $timeout(function () {
                                tooltipElement.removeClass('showTooltip')
                            });
                        }

                    });

                }, delay);


                function setPosition() {
                    var mouseX = event.clientX,
                        mouseY = event.clientY,
                        offsetX = offsetY = 5,
                        positionX, positionY;

                    var pos = element[0].getBoundingClientRect();
                    positionY = pos.top - tooltipElement.outerHeight(true) - 5;

                    var offset = $(window).width() - (pos.left + tooltipElement.outerWidth(true));
                    if (offset < 0) {
                        positionX = pos.left + offset;
                    } else {
                        positionX = pos.left;
                    }


                    tooltipElement.css({ top: positionY, left: positionX });
                }


            });
            element.on('mouseleave', function () {
                $timeout.cancel(hoverIntentPromise);

                leaveIntentPromise = $timeout(function () {
                    tooltipElement.removeClass('showTooltip');
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