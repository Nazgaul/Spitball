
module app {
    "use strict";

    class CartAnimation implements angular.IDirective {
        constructor(
            private $animate: angular.animate.IAnimateService
        ) {
        }
        // scope = true;
        restrict = 'A';
        priority = -1;
        link = (scope: angular.IScope, element: JQuery, attrs: ng.IAttributes) => {
            element.click(
                () => {
                    var strId = element[0].id.replace("box_", "");
                    var id = parseInt(strId, 10);
                    var cart = $(attrs['cartAnimationTo']);
                    if (!cart.length) {

                        scope["cc"].animationEnd(id);
                        return;
                    }
                    var dest = cart.offset();
                    var elem = $(element).find(attrs['cartAnimation']);
                    var color = elem.parents('[color-parent]').attr('class').replace("-parent","");
                    var offeset = elem.offset();
                    var elemtClone = elem.clone().addClass("angular-animate cart-animated " + color)
                        .css({
                            'position': 'absolute',
                            'top': offeset.top,
                            'left': offeset.left
                        })
                        .appendTo($('body'));
                    this.$animate.animate(elemtClone,
                        {
                            transform: "translate(" + 0 + 'px,' + 0 + "px)"
                        },
                        {
                            transform: "translate(" + (dest.left - offeset.left) + "px," + (dest.top - offeset.top) + "px)",
                            opacity: 0.3
                        })
                        .then(() => {
                            elemtClone.remove();
                            scope["cc"].animationEnd(id);

                        });

                });
        }

        static factory(): angular.IDirectiveFactory {
            const directive = ($animate) => {
                return new CartAnimation($animate);
            };
            directive["$inject"] = ["$animate"];
            return directive;
        }
    }
    angular
        .module("app")
        .directive("cartAnimation", CartAnimation.factory());
}