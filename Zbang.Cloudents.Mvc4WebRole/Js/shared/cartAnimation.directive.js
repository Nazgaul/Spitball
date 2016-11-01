var app;
(function (app) {
    "use strict";
    var CartAnimation = (function () {
        function CartAnimation($timeout, $animate) {
            var _this = this;
            this.$timeout = $timeout;
            this.$animate = $animate;
            this.restrict = 'A';
            this.link = function (scope, element, attrs) {
                element.on('click', function () {
                    var cart = $(attrs['cartAnimationTo']);
                    if (!cart.length) {
                        return;
                    }
                    var dest = cart.offset();
                    var elem = $(element).find(attrs['cartAnimation']);
                    var elemtClone = elem.clone().addClass("angular-animate cart-animated")
                        .css({
                        'position': 'absolute'
                    })
                        .appendTo($('body'));
                    var offeset = elem.offset();
                    console.log(offeset, cart.offset());
                    _this.$animate.animate(elemtClone, {
                        top: offeset.top,
                        left: offeset.left,
                        opacity: 1
                    }, {
                        top: dest.top,
                        left: dest.left,
                        opacity: 0.3
                    }).then(function () {
                        elemtClone.remove();
                    });
                });
            };
        }
        CartAnimation.factory = function () {
            var directive = function ($timeout, $animate) {
                return new CartAnimation($timeout, $animate);
            };
            directive["$inject"] = ["$timeout", "$animate"];
            return directive;
        };
        return CartAnimation;
    }());
    angular
        .module("app")
        .directive("cartAnimation", CartAnimation.factory());
})(app || (app = {}));
