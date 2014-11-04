
app.directive('fbBlock',
    [
        function () {
            "use strict";
            return {
                restrict: "A",
                scope: false,
                link: function (scope, elem, attrs) {
                    
                    var fbDialog,element,fInterval, interval;

                    if (!fbDialog) {
                        fInterval = setInterval(function () {
                            fbDialog = angular.element('.fb_dialog iframe').closest('.fb_dialog');

                            if (fbDialog.length) {
                                clearInterval(fInterval);
                                return;
                            }

                            angular.element(document.body).append(element);
                        }, 50);
                    }
                    interval = setInterval(function () {
                        if (fbDialog.position().top > 0) {
                            clearInterval(interval);
                            element = angular.element('<div></div>');
                            element.css({
                                width: fbDialog.outerWidth(),
                                height: 90,
                                position: 'absolute',
                                zIndex: 999999,
                                opacity: 1,
                                top: fbDialog.position().top,
                                left: fbDialog.position().left
                            });
                        }

                        angular.element(document.body).append(element);
                    }, 50);


                    scope.$on('$destroy', function () {
                        element.remove();
                    });
                    
                }
            };
        }
    ]);