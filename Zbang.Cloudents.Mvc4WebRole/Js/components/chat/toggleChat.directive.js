var app;
(function (app) {
    var ToggleChat = (function () {
        function ToggleChat(chatBus) {
            var _this = this;
            this.chatBus = chatBus;
            this.restrict = 'A';
            this.link = function (scope, element, attrs) {
                angular.element(element).on('click', function () {
                    $('.page-body').toggleClass('expanded-chat');
                });
                scope.$on('expandChat', function () {
                    $('.page-body').addClass('expanded-chat');
                });
                var counterElem = $('.chat-counter');
                scope.$watch(_this.chatBus.getUnread, function (value) {
                    if (value > 0) {
                        counterElem.text(value.toString()).show();
                    }
                    else {
                        counterElem.hide();
                    }
                });
            };
        }
        ToggleChat.factory = function () {
            var directive = function (chatBus) {
                return new ToggleChat(chatBus);
            };
            directive['$inject'] = ['chatBus'];
            return directive;
        };
        return ToggleChat;
    }());
    angular
        .module("app.chat")
        .directive("toggleChat", ToggleChat.factory());
})(app || (app = {}));
