var app;
(function (app) {
    "use strict";
    var CartAnimation = (function () {
        function CartAnimation($timeout) {
            var _this = this;
            this.$timeout = $timeout;
            this.restrict = 'A';
            this.link = function (scope, element, attrs) {
                element.on('click', function () {
                    var placeHolder = $('#' + attrs['cartAnimationTo'] + ' .empty:first');
                    var cart = placeHolder.length ? placeHolder : $('#' + attrs['cartAnimationTo'] + ' .icon:last-of-type');
                    var destTop = cart.offset().top;
                    var destLeft = cart.offset().left;
                    var elem = $(element).find('.' + attrs['cartAnimation']);
                    var elemtClone = elem.clone().css({
                        'position': 'absolute',
                        'top': elem.offset().top,
                        'left': elem.offset().left,
                        'opacity': 1
                    }).appendTo($('body'));
                    _this.$timeout(function () {
                        var timing = "0.5s";
                        var style = elemtClone.attr('style') +
                            'top: ' + destTop + 'px; ' +
                            'left: ' + destLeft + 'px; ' +
                            'opacity: 0.3;' +
                            'transition: top ' + timing + ' linear' + ', left ' + timing + ' linear, opacity ' + timing + ' linear';
                        elemtClone.attr('style', style).addClass('cart-animated');
                        _this.$timeout(function () {
                            elemtClone.remove();
                        }, 500);
                    }, 0);
                });
            };
        }
        CartAnimation.factory = function () {
            var directive = function ($timeout) {
                return new CartAnimation($timeout);
            };
            directive["$inject"] = ["$timeout"];
            return directive;
        };
        return CartAnimation;
    }());
    angular
        .module("app")
        .directive("cartAnimation", CartAnimation.factory());
})(app || (app = {}));
