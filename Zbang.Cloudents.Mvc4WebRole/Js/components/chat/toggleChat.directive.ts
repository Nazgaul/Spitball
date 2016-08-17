module app {
    class ToggleChat implements angular.IDirective {
        restrict = 'A';

        constructor(private chatBus) {
        }
        link = (scope: IChatTimeAgo, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
            angular.element(element).on('click', () => {
                $('html').toggleClass('expanded-chat');

            });
            scope.$on('expandChat', function () {
                $('html').addClass('expanded-chat');
            });

            var counterElem = $('.chat-counter');
            scope.$watch(this.chatBus.getUnread
                , (value) => {
                    if (value > 0) {
                        counterElem.text(value.toString()).show();
                    }
                    else {
                        counterElem.hide();
                    }
                });
        };


        public static factory(): angular.IDirectiveFactory {
            var directive = (chatBus) => {
                return new ToggleChat(chatBus);
            };

            directive['$inject'] = ['chatBus'];

            return directive;
        }
    }

    angular
        .module("app.chat")
        .directive("toggleChat", ToggleChat.factory());
}