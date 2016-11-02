
module app {
    "use strict";

    class CartAnimation implements angular.IDirective {
        constructor(
            private $animate: angular.animate.IAnimateService
        ) {
        }

        restrict = 'A';
        priority = -1;
        link = (scope, element: JQuery, attrs: ng.IAttributes) => {
            element.click(
                () => {
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
                    //var timing = "0.5s";
                    var offeset = elem.offset();
                    this.$animate.animate(elemtClone,
                        {
                            //position: 'absolute',
                            top: offeset.top,
                            left: offeset.left,
                            opacity: 1
                        },
                        {
                            top: dest.top,
                            left: dest.left,
                            opacity: 0.3
                        }).then(() => {
                            elemtClone.remove();
                    });
                    //elemtClone.bind("transitionend",
                    //    () => {
                    //        elemtClone.remove();
                    //    });

                    //this.$timeout(() => {


                    //    var style = elemtClone.attr('style') +
                    //        'top: ' +
                    //        destTop +
                    //        'px; ' +
                    //        'left: ' +
                    //        destLeft +
                    //        'px; ' +
                    //        'opacity: 0.3;' +
                    //        'transition: top ' +
                    //        timing +
                    //        ' linear' +
                    //        ', left ' +
                    //        timing +
                    //        ' linear, opacity ' +
                    //        timing +
                    //        ' linear';
                    //    elemtClone.attr('style', style).addClass('cart-animated');

                    //    //this.$timeout(() => {
                    //    //    elemtClone.remove();
                    //    //}, 500)
                    //});

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
//(function () {
//    'use strict';

//    angular.module('app').directive('cartAnimation', cartAnimation);
//    cartAnimation.$inject = ['$timeout'];
//    function cartAnimation($timeout) {
//        return {
//            restrict: 'A',
//            link: function (scope, element, attrs) {
//                element.on('click', function () {
//                    var placeHolder = $('#' + attrs.cartAnimationTo + ' .empty:first');
//                    var cart = placeHolder.length ? placeHolder : $('#' + attrs.cartAnimationTo + ' .icon:last-of-type');
//                    var destTop = cart.offset().top;
//                    var destLeft = cart.offset().left;
//                    var elem = $(element).find('.' + attrs.cartAnimation);
//                    var elemtClone = elem.clone().css({
//                        'position': 'absolute',
//                        'top': elem.offset().top,
//                        'left': elem.offset().left,
//                        'opacity': 1
//                    }).appendTo($('body'));

//                    $timeout(function () {
//                        var timing = "0.5s";
//                        var style = elemtClone.attr('style') +
//                        'top: ' + destTop + 'px; ' +
//                        'left: ' + destLeft + 'px; ' +
//                        'opacity: 0.3;' +
//                        'transition: top ' + timing + ' linear' + ', left ' + timing + ' linear, opacity ' + timing + ' linear';
//                        elemtClone.attr('style', style).addClass('cart-animated');
//                        $timeout(function () {
//                            elemtClone.remove();
//                        }, 500)
//                    }, 0)

//                })
//            }
//        };
//    }
//})();