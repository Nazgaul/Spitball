var app;
(function (app) {
    "use strict";
    var CardScroll = (function () {
        function CardScroll($mdMedia, $anchorScroll, $timeout) {
            var _this = this;
            this.$mdMedia = $mdMedia;
            this.$anchorScroll = $anchorScroll;
            this.$timeout = $timeout;
            this.restrict = "A";
            this.link = function (scope, element, attrs) {
                if (!_this.$mdMedia("xs")) {
                    return;
                }
                function getCoord(e, c) {
                    return /touch/.test(e.type) ? (e.originalEvent || e).changedTouches[0]['page' + c] : e['page' + c];
                }
                var ctrl = scope["f"];
                var startY;
                $(window)
                    .on("touchmove", function (ev) {
                    ev.preventDefault();
                });
                $(window)
                    .on('touchstart', function (ev) {
                    startY = getCoord(ev, 'Y');
                })
                    .on('touchend', function (ev) {
                    ev.preventDefault();
                    if (startY > getCoord(ev, 'Y')) {
                        ctrl.next();
                        _this.$anchorScroll("card" + ctrl.slidepos);
                    }
                    else {
                        console.log(startY, getCoord(ev, 'Y'), "up");
                        ctrl.prev();
                        _this.$anchorScroll("card" + ctrl.slidepos);
                    }
                });
                scope.$on("$destroy", function () {
                    $(window).unbind("touchstart touchend touchmove");
                });
            };
        }
        CardScroll.factory = function () {
            var directive = function ($mdMedia, $anchorScroll, $timeout) {
                return new CardScroll($mdMedia, $anchorScroll, $timeout);
            };
            directive['$inject'] = ['$mdMedia', "$anchorScroll", "$timeout"];
            return directive;
        };
        return CardScroll;
    }());
    angular
        .module("app.flashcard")
        .directive("cardScroll", CardScroll.factory());
})(app || (app = {}));
