app.directive('fbBlock',
    [
        function () {
            return {
                restrict: "A",
                scope: false,
                link: function (scope, elem, attrs) {
                    
                    var fbDialog = angular.element('.fb_dialog  iframe').closest('.fb_dialog'),
                        element,
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