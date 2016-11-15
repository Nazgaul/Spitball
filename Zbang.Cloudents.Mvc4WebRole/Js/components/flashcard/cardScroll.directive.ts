module app {
    "use strict";
    class CardScroll implements angular.IDirective {
        constructor(private $mdMedia: angular.material.IMedia,
            private $anchorScroll: angular.IAnchorScrollService,
            private $timeout: angular.ITimeoutService) {

        }

        restrict = "A";
        link = (scope: angular.IScope, element: JQuery, attrs: ng.IAttributes) => {
            if (!this.$mdMedia("xs")) {
                return;
            }
            function getCoord(e, c) {
                return /touch/.test(e.type) ? (e.originalEvent || e).changedTouches[0]['page' + c] : e['page' + c];
            }
            var ctrl: FlashcardController = scope["f"];

            var startY;
            $(window)
                .on("touchmove",
                (ev) => {
                    ev.preventDefault();
                });
            $(window)
                .on('touchstart',
                    ev => {
                        startY = getCoord(ev, 'Y');

                    })
                .on('touchend',
                ev => {
                    ev.preventDefault();
                    if (startY > getCoord(ev, 'Y')) {
                        //down
                        ctrl.next();
                        this.$anchorScroll("card" + ctrl.slidepos);
                    } else {
                        console.log(startY, getCoord(ev, 'Y'),"up");
                        ctrl.prev();
                        this.$anchorScroll("card" + ctrl.slidepos);
                    }
                });
            //var x: angular.IPromise<void>;
            //var lastScrollTop = 0;
            //$(window)
            //    .scroll(() => {
            //        if (x) {
            //            console.log("cancel");
            //            this.$timeout.cancel(x);
            //        }
            //        x = this.$timeout(() => {
            //            var st = $(window).scrollTop();
            //            if (st > lastScrollTop) {
            //                console.log("down");
            //                ctrl.next();
            //                this.$anchorScroll("card" + ctrl.slidepos);
            //                lastScrollTop = $(window).scrollTop();
            //            } else {
            //                console.log("up");
            //                ctrl.prev();
            //                this.$anchorScroll("card" + ctrl.slidepos);
            //                lastScrollTop = $(window).scrollTop();
            //            }
            //        },
            //            10);

            //    });
            //scope.$on("$destroy",
            //    () => {
            //        $(window).unbind("touchstart touchend");
            //    });


        }
        static factory(): angular.IDirectiveFactory {

            const directive = ($mdMedia, $anchorScroll, $timeout) => {
                return new CardScroll($mdMedia, $anchorScroll, $timeout);
            };
            directive['$inject'] = ['$mdMedia', "$anchorScroll", "$timeout"];
            return directive;
        }
    }
    angular
        .module("app.flashcard")
        .directive("cardScroll", CardScroll.factory());
}