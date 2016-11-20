module app {
    "use strict";
    class CardScroll implements angular.IDirective {
        constructor(private $mdMedia: angular.material.IMedia,
            private $anchorScroll: angular.IAnchorScrollService,
            private $timeout: angular.ITimeoutService) {

        }

        restrict = "A";
        link = (scope: angular.IScope, element: JQuery) => {
            if (!this.$mdMedia("xs")) {
                return;
            }
            function getCoord(e) {
                return /touch/.test(e.type) ? (e.originalEvent || e).changedTouches[0]["pageY"] : e["pageY"];
            }
            var ctrl: FlashcardController = scope["f"];

            var startY;
            $(element)
                .on("touchmove",
                (ev) => {
                    if (ev.target.scrollHeight > ev.target.clientHeight) {
                        // if the element has scroll on it.
                        return;
                    }
                    ev.preventDefault();
                });
            $(element)
                .on("touchstart",
                    ev => {
                        startY = getCoord(ev);

                    })
                .on("touchend",
                ev => {
                    if (Math.abs(startY - getCoord(ev)) < 20) {
                        // click was made
                        return;
                    }
                    //console.log(ev, ev.target.scrollHeight, ev.target.clientHeight);
                    var target = ev.target, jTarget = $(target);
                    
                    if (target.scrollHeight > target.clientHeight) {
                        if (jTarget.scrollTop() + jTarget.innerHeight() < jTarget[0].scrollHeight) {
                            // be able to scroll to the end of the element
                            return;
                        } 
                       
                    }
                    this.$anchorScroll.yOffset = 150; 
                    ev.preventDefault();
                    if (startY > getCoord(ev)) {
                        ctrl.next();
                        this.$anchorScroll("card" + ctrl.slidepos);
                    } else {
                        ctrl.prev();
                        this.$anchorScroll("card" + ctrl.slidepos);
                    }
                });
            scope.$on("$destroy",
                () => {
                    $(window).unbind("touchstart touchend touchmove");
                });


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