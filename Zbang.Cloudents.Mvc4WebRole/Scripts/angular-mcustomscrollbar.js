angular.module('custom_scrollbar', []).directive('mCustomScrollbar', ['$timeout', function ($timeout) {    
    return {
        restrict: 'A',
        transclude: true,
        template: '<div><div ng-transclude></div></div>',
        replace: true,
        link: function ($scope, $elem, $attr) {
            var $win = $(window), height = $attr.height, top, bottom;

            setScroll();

            $scope.$on('update-scroll', updateScroll);

            $win.resize(updateScroll);

            function updateScroll() {
                $timeout(function() {
                    $elem.mCustomScrollbar('destroy');
                    setScroll();
                }, 50);
            }

            function setScroll() {
                calcHeight();
                //m-custom-scrollbar-class
                $elem.mCustomScrollbar({
                    setHeight: height,
                    theme: "dark",
                    advanced: { updateOnContentResize: false },
                    callbacks: {
                        onScroll: function () {
                            if (this.mcs.topPct > 75) {
                                if ($attr.mCustomScrollbar) {
                                    $scope.$apply($attr.mCustomScrollbar);
                                }
                            }
                        },
                        whileScrolling: function () {
                            if (this.mcs.topPct === 0) {
                                $elem.removeClass($attr.mCustomScrollbarClass);
                            }
                        },
                        whileScrolling:function() {
                            $elem.addClass($attr.mCustomScrollbarClass);
                        }
                    }
                });
                
            }

            function calcHeight() {
                if ($attr.height) {
                    return;
                }
                top = $attr.top ? parseInt($attr.top) : $elem[0].getBoundingClientRect().top;
                bottom = $attr.bottom ? parseInt($attr.bottom) : 0;
                height = $win.height() - (top + bottom);
            }

            $scope.$on('$destroy', function () {
                $elem.mCustomScrollbar('destroy');
            });
            $scope.$on('$routeChangeStart', function () {
                $elem.mCustomScrollbar('destroy');
            });
        }
    };
}]);


