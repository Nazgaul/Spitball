var app;
(function (app) {
    "use strict";
    var SbScroll = (function () {
        function SbScroll($compile, $mdMedia) {
            var _this = this;
            this.$compile = $compile;
            this.$mdMedia = $mdMedia;
            this.restrict = 'A';
            this.terminal = true;
            this.priority = 1000;
            this.link = function (scope, element, attribute) {
                element.removeAttr('sb-scroll');
                if (_this.$mdMedia('xs')) {
                    element.removeAttr('container');
                }
                if (Modernizr.cssscrollbar) {
                    element.removeAttr("ng-scrollbars-config ng-scrollbars-paging-function ng-scrollbars");
                    _this.$compile(element)(scope);
                    return;
                }
                element.removeAttr("srph-infinite-scroll");
                _this.$compile(element)(scope);
            };
        }
        SbScroll.factory = function () {
            var directive = function ($compile, $mdMedia) {
                return new SbScroll($compile, $mdMedia);
            };
            directive["$inject"] = ["$compile", "$mdMedia"];
            return directive;
        };
        return SbScroll;
    }());
    angular
        .module("app")
        .directive("sbScroll", SbScroll.factory());
})(app || (app = {}));
(function (app) {
    "use strict";
    var ChatScrollButtom = (function () {
        function ChatScrollButtom($mdMedia) {
            var _this = this;
            this.$mdMedia = $mdMedia;
            this.link = function (scope, element, attribute) {
                scope.$on('chat-scroll', function () {
                    if (_this.$mdMedia('xs')) {
                        window.scrollTo(0, document.body.scrollHeight);
                    }
                    if (Modernizr.cssscrollbar) {
                        element[0].scrollTop = element[0].scrollHeight;
                        return;
                    }
                    scope["c"].updateScrollbar('scrollTo', 'bottom', { scrollInertia: 0, timeout: 100 });
                });
            };
        }
        ChatScrollButtom.factory = function () {
            var directive = function ($mdMedia) {
                return new ChatScrollButtom($mdMedia);
            };
            directive["$inject"] = ["$mdMedia"];
            return directive;
        };
        return ChatScrollButtom;
    }());
    angular
        .module("app")
        .directive("chatScrollButtom", ChatScrollButtom.factory());
})(app || (app = {}));
//# sourceMappingURL=sbScroll.directive.js.map