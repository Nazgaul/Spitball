
app.directive('userTooltipPopup', ['$timeout', '$templateCache', '$compile', 'sUser', 'sUserDetails','sModal',
function ($timeout, $templateCache, $compile, sUser, sUserDetails, sModal) {
    "use strict";

    var tooltipTemplate = $templateCache.get('userToolTip.html'),
        showTooltip = 'showTooltip'
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
                        scope.user = response;
                        $compile(tooltipElement)(scope);
                    });

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
                        },
                        click: function () {
                            $timeout(function () {
                                tooltipElement.removeClass(showTooltip);
                            });
                        }

                    });

                }, delay);


                function setPosition() {
                    var positionX, positionY, pos, offset,
                    isLtr = $('html').css('direction') === 'ltr';

                    pos = element[0].getBoundingClientRect();
                    positionY = pos.top - tooltipElement.outerHeight(true) - 5;

                    if (isLtr) {
                        offset = $(window).width() - (pos.left + tooltipElement.outerWidth(true));
                        if (offset < 0) {
                            positionX = pos.left + offset;
                        } else {
                            positionX = pos.left;
                        }
                    } else {
                        offset = pos.left - tooltipElement.outerWidth(true);
                        if (offset < 0) {
                            positionX = 0;
                        } else {
                            positionX = pos.left - tooltipElement.outerWidth(true) + pos.width;
                        }
                    }

                    tooltipElement.css({ top: positionY, left: positionX });
                }


            });

            element.on('mouseleave', function () {
                $timeout.cancel(hoverIntentPromise);

                leaveIntentPromise = $timeout(function () {
                    tooltipElement.removeClass(showTooltip);
                }, 250);

            });

            scope.sendUserMessage = function (user) {
                sModal.open('shareEmail', { 
                    data: {
                        singleMessage: true,
                        users: [user]
                    }
                });
            };

            scope.$on('$destroy', function () {
                if (tooltipElement) {
                    tooltipElement.remove();
                }
            });
        }
    };
}]);