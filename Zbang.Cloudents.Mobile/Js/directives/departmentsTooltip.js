
app.directive('departmentTooltipPopup', ['$timeout', '$templateCache', '$compile', 'sUser',
    function ($timeout, $templateCache, $compile, sUser) {
        "use strict";
        var tooltipTemplate = $templateCache.get('memberDepartmentToolTip.html');
        return {
            restrict: 'A',            
            link: function (scope, element, attributes) {

                    var hoverIntentPromise,
                    leaveIntentPromise,
                    appendToBody = (attributes.departmentAppendToBody === 'true') ? true : false,
                    userId = parseInt(attributes.departmentUserId, 10),
                    $body = angular.element(document.body),
                    delay = 500,
                    tooltipElement;


                if (!userId) {
                    return;
                }

                if (!tooltipTemplate) {
                    return;
                }

                element.on('mouseenter', function (event) {
                    if (!tooltipElement) {
                        tooltipElement = angular.element(tooltipTemplate);

                        if (appendToBody) {
                            $body.append(tooltipElement);
                        } else {
                            element.append(tooltipElement);
                        }

                        sUser.departments({ userId: userId }).then(function (data) {
                            scope.departments = data;
                            $compile(tooltipElement)(scope);
                        });


                    }

                    hoverIntentPromise = $timeout(function () {
                        setPosition();
                        tooltipElement.fadeIn({ duration: 300 });

                        tooltipElement.on({
                            mouseenter: function () {
                                $timeout.cancel(leaveIntentPromise);
                            },
                            mouseleave: function () {
                                tooltipElement.hide();
                            },
                            click: function () {
                                $timeout(function () {
                                    tooltipElement.hide();
                                });
                            }

                        });

                    }, delay);


                    function setPosition() {
                        var positionX, positionY, pos, offset,
                        isLtr = $('html').css('direction') === 'ltr';

                        pos = element[0].getBoundingClientRect();
                        positionY = pos.top - tooltipElement.outerHeight(true) - 5;

                        positionX = pos.left - tooltipElement.outerWidth(true) /  2 + pos.width / 2;

                        tooltipElement.css({ top: positionY, left: positionX });
                    }


                });
                element.on('mouseleave', function () {
                    $timeout.cancel(hoverIntentPromise);

                    leaveIntentPromise = $timeout(function () {
                        tooltipElement.hide();
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