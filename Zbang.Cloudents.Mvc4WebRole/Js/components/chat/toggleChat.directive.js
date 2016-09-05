var app;
(function (app) {
    var ToggleChat = (function () {
        function ToggleChat(chatBus, $mdMedia, userDetailsFactory, $rootScope, $state) {
            var _this = this;
            this.chatBus = chatBus;
            this.$mdMedia = $mdMedia;
            this.userDetailsFactory = userDetailsFactory;
            this.$rootScope = $rootScope;
            this.$state = $state;
            this.restrict = 'A';
            this.link = function (scope, element) {
                var $html = $('html');
                var className = 'expanded-chat';
                if (!_this.userDetailsFactory.getUniversity()) {
                    element.addClass('hidden');
                }
                _this.$rootScope.$on('change-university', function () {
                    if (_this.userDetailsFactory.getUniversity()) {
                        element.removeClass('hidden');
                    }
                });
                element.on('click', function () {
                    if (_this.$mdMedia('xs')) {
                        _this.$state.go("chat");
                        return;
                    }
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
            var directive = function (chatBus, $mdMedia, userDetailsFactory, $rootScope, $state) {
                return new ToggleChat(chatBus, $mdMedia, userDetailsFactory, $rootScope, $state);
            };
            directive['$inject'] = ['chatBus', '$mdMedia', 'userDetailsFactory', '$rootScope', "$state"];
            return directive;
        };
        return ToggleChat;
    }());
    angular
        .module("app.chat")
        .directive("toggleChat", ToggleChat.factory());
})(app || (app = {}));
//# sourceMappingURL=toggleChat.directive.js.map