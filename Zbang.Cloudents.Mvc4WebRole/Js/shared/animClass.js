(function () {
    angular.module('app').directive('animationClass', animClass);
    animClass.$inject = ['$state'];

    function animClass($state) {
        return {
            link: function(scope, elem) {

                if (scope.app.clickLocation) {
                    var x = scope.app.clickLocation.x, y = scope.app.clickLocation.y;
                    elem.css({
                        '-moz-transform-origin': x + ' ' + y,
                        '-ms-transform-origin': x + ' ' + y,
                        '-webkit-transform-origin': x + ' ' + y,
                        'transform-origin': x + ' ' + y,

                    });
                    scope.app.clickLocation = null;
                }
                if ($state.current.data) {
                    var enterClass = $state.current.data.animateClass;
                    elem.addClass(enterClass);
                }
                scope.$on('$destroy', function() {
                    elem.removeClass(enterClass);
                    if ($state.current.data) {
                        elem.addClass($state.current.data.animateClass);
                    }
                });
            }
        };
    }
})();

(function() {
    angular.module('app').directive('animationLocation', animationLocation);

    function animationLocation() {
        return {
            link: function(scope, elem) {
                $(elem).on('click', 'a', function(e) {
                    var xPos = e.clientX + 'px', yPos = e.clientY + 'px';
                    scope.app.clickLocation = {
                        x: xPos,
                        y: yPos
                    };
                    //$('[animation-class]').css({
                    //    '-moz-transform-origin': xPos + ' ' + yPos,
                    //    '-ms-transform-origin': xPos + ' ' + yPos,
                    //    '-webkit-transform-origin': xPos + ' ' + yPos,
                    //    'transform-origin': xPos + ' ' + yPos,

                    //});
                    console.log(e);
                });
            }
        };
    }
})();