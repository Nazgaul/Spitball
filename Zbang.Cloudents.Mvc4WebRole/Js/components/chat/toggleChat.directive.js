var app;
(function (app) {
    var ToggleChat = (function () {
        function ToggleChat(chatBus, $mdMedia) {
            var _this = this;
            this.chatBus = chatBus;
            this.$mdMedia = $mdMedia;
            this.restrict = 'A';
            this.link = function (scope, element) {
                var $pageBody = $('.page-body');
                var className = 'expanded-chat';
                element.click(function () {
                    $pageBody.toggleClass(className);
                });
                scope.$on('expandChat', function () {
                    $pageBody.addClass(className);
                });
                if (!_this.$mdMedia('xs')) {
                    return;
                }
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
            var directive = function (chatBus, $mdMedia) {
                return new ToggleChat(chatBus, $mdMedia);
            };
            directive['$inject'] = ['chatBus', '$mdMedia'];
            return directive;
        };
        return ToggleChat;
    }());
    angular
        .module("app.chat")
        .directive("toggleChat", ToggleChat.factory());
})(app || (app = {}));
