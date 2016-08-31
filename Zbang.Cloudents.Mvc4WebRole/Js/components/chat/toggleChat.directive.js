var app;
(function (app) {
    var ToggleChat = (function () {
        function ToggleChat(chatBus, $mdMedia, userDetailsFactory, $rootScope) {
            var _this = this;
            this.chatBus = chatBus;
            this.$mdMedia = $mdMedia;
            this.userDetailsFactory = userDetailsFactory;
            this.$rootScope = $rootScope;
            this.restrict = 'A';
            this.link = function (scope, element) {
                var $html = $('html');
                var className = 'expanded-chat';
                if (!_this.userDetailsFactory.getUniversity()) {
                    element.hide();
                }
                _this.$rootScope.$on('change-university', function () {
                    if (_this.userDetailsFactory.getUniversity()) {
                        element.show();
                    }
                });
                element.on('click', function () {
                    $html.toggleClass(className);
                });
                scope.$on('expandChat', function () {
                    $html.addClass(className);
                });
                if (_this.$mdMedia('gt-sm')) {
                    return;
                }
                var counterElem = $('.chat-counter');
                var cleanUpFunc = scope.$watch(_this.chatBus.getUnread, function (value) {
                    if (value > 0) {
                        counterElem.text(value.toString()).show();
                    }
                    else {
                        counterElem.hide();
                    }
                });
                scope.$on("$destroy", function () {
                    cleanUpFunc();
                });
            };
        }
        ToggleChat.factory = function () {
            var directive = function (chatBus, $mdMedia, userDetailsFactory, $rootScope) {
                return new ToggleChat(chatBus, $mdMedia, userDetailsFactory, $rootScope);
            };
            directive['$inject'] = ['chatBus', '$mdMedia', 'userDetailsFactory', '$rootScope'];
            return directive;
        };
        return ToggleChat;
    }());
    angular
        .module("app.chat")
        .directive("toggleChat", ToggleChat.factory());
})(app || (app = {}));
//# sourceMappingURL=toggleChat.directive.js.map