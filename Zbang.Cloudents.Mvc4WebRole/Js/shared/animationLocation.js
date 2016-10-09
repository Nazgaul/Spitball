(function () {
    'use strict';
    angular.module('app').directive('animationLocation', animationLocation);

    function animationLocation() {
        return {
            link: function (scope, elem) {
                $(elem).on('click', 'a', function (e) {
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
                });
            }
        };
    }
})();