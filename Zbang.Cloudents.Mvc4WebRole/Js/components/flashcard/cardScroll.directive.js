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
            this.link = function (scope, element) {
                if (!_this.$mdMedia("xs")) {
                    return;
                }
                function getCoord(e) {
                    return /touch/.test(e.type) ? (e.originalEvent || e).changedTouches[0]["pageY"] : e["pageY"];
                }
                var ctrl = scope["f"];
                var startY;
                $(element)
                    .on("touchmove", function (ev) {
                    if (ev.target.scrollHeight > ev.target.clientHeight) {
                        return;
                    }
                    ev.preventDefault();
                });
                $(element)
                    .on("touchstart", function (ev) {
                    startY = getCoord(ev);
                })
                    .on("touchend", function (ev) {
                    if (Math.abs(startY - getCoord(ev)) < 20) {
                        return;
                    }
                    var target = ev.target, jTarget = $(target);
                    if (target.scrollHeight > target.clientHeight) {
                        if (jTarget.scrollTop() + jTarget.innerHeight() < jTarget[0].scrollHeight) {
                            return;
                        }
                    }
                    _this.$anchorScroll.yOffset = 150;
                    ev.preventDefault();
                    if (startY > getCoord(ev)) {
                        ctrl.next();
                        _this.$anchorScroll("card" + ctrl.slidepos);
                    }
                    else {
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
