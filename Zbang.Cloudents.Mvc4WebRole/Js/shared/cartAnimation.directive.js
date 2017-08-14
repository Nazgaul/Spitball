var app;
(function (app) {
    "use strict";
    var CartAnimation = (function () {
        function CartAnimation($animate) {
            var _this = this;
            this.$animate = $animate;
            this.restrict = 'A';
            this.priority = -1;
            this.link = function (scope, element, attrs) {
                element.click(function () {
                    var strId = element[0].id.replace("box_", "");
                    var id = parseInt(strId, 10);
                    var cart = $(attrs['cartAnimationTo']);
                    if (!cart.length) {
                        scope["cc"].animationEnd(id);
                        return;
                    }
                    var dest = cart.offset();
                    var elem = $(element).find(attrs['cartAnimation']);
                    var color = elem.parents('[color-parent]').attr('class').replace("-parent", "");
                    var offeset = elem.offset();
                    var elemtClone = elem.clone().addClass("angular-animate cart-animated " + color)
                        .css({
                        'position': 'absolute',
                        'top': offeset.top,
                        'left': offeset.left
                    })
                        .appendTo($('body'));
                    _this.$animate.animate(elemtClone, {
                        transform: "translate(" + 0 + 'px,' + 0 + "px)"
                    }, {
                        transform: "translate(" + (dest.left - offeset.left) + "px," + (dest.top - offeset.top) + "px)",
                        opacity: 0.3
                    })
                        .then(function () {
                        elemtClone.remove();
                        scope["cc"].animationEnd(id);
                    });
                });
            };
        }
        CartAnimation.factory = function () {
            var directive = function ($animate) {
                return new CartAnimation($animate);
            };
            directive["$inject"] = ["$animate"];
            return directive;
        };
        return CartAnimation;
    }());
    angular
        .module("app")
        .directive("cartAnimation", CartAnimation.factory());
})(app || (app = {}));
//# sourceMappingURL=cartAnimation.directive.js.map