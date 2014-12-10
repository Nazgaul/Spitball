angular.module('custom_scrollbar', []).directive('mCustomScrollbar', ['$timeout', function ($timeout) {
    return {
        restrict: 'A',
        transclude: true,
        scope: {
            scrollDisabled: '=',
            addItems: '&'
        },
        template: '<div><div ng-transclude></div></div>',
        replace: true,
        link: function ($scope, $elem, $attr) {
            var $win = $(window), height = $attr.height, top, bottom, lastHeight, updateEvent, resizeEvent;

            $scope.$watch('scrollDisabled', function (value) {
                if (value) {
                    return;
                }

                $timeout(function () {
                    setScroll();
                    setEvents();
                }, 40);
            });

            $scope.$on('$destroy', function () {
                $elem.mCustomScrollbar('destroy');
                updateEvent();
                resizeEvent();
            });
            $scope.$on('$routeChangeStart', function () {
                $elem.mCustomScrollbar('destroy');
            });


            if (!$scope.scrollDisabled) {
                setScroll();
                setEvents();
            }



            function setEvents() {
                updateEvent = $scope.$on('update-scroll', updateScroll);
                resizeEvent = $scope.$on('elastic:resize', updateScroll);
                $win.resize(updateScroll);
            }

            function updateScroll(e, fixHeight) {

                if (_.isNumber(fixHeight) && fixHeight >= 0) {
                    lastHeight = $elem.height();
                    $elem.height(fixHeight);
                    return;
                }

                calcHeight();
                $elem.height(height);
                setTimeout(function () {
                    $elem.mCustomScrollbar('update');
                    //setScroll();
                }, 250);
            }

            function setScroll() {
                calcHeight();
                //m-custom-scrollbar-class
                $elem.mCustomScrollbar({
                    setHeight: height,
                    theme: "dark",
                    advanced: { updateOnContentResize: false },
                    callbacks: {
                        onTotalScrollOffset: 600,
                        onTotalScroll: function () {
                            $scope.$apply(function () {
                                $scope.addItems();
                            });
                            
                        },
                        whileScrolling: function () {

                            if (this.mcs.topPct === 0) {
                                $elem.removeClass($attr.mCustomScrollbarClass);
                                return;
                            }
                            $elem.addClass($attr.mCustomScrollbarClass);
                        }
                    }
                });

            }

            function calcHeight() {
                if ($attr.height) {
                    return;
                }

                if (lastHeight) {
                    height = lastHeight;
                    lastHeight = null;
                    return;
                }
                var rect = $elem[0].getBoundingClientRect();
                top = $attr.top ? parseInt($attr.top) : rect.top;
                bottom = $attr.bottom ? parseInt($attr.bottom) : 0;
                height = $win.height() - (top + bottom);


            }


        }
    };
}]);


