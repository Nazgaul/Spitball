module app {
    class ToggleChat implements angular.IDirective {
        restrict = 'A';

        constructor(private chatBus, private $mdMedia: angular.material.IMedia) {
        }
        link = (scope: IChatTimeAgo, element: ng.IAugmentedJQuery) => {
            var $pageBody = $('.page-body');
            const className = 'expanded-chat';
            element.click(() => {
                $pageBody.toggleClass(className);
            });
            scope.$on('expandChat', () => {
                $pageBody.addClass(className);
            });
            if (!this.$mdMedia('xs')) {
                return;
            }
            var counterElem = $('.chat-counter');
            scope.$watch(this.chatBus.getUnread,
                (value) => {
                    if (value > 0) {
                        counterElem.text(value.toString()).show();
                    } else {
                        counterElem.hide();
                    }
                });

        };


        public static factory(): angular.IDirectiveFactory {
            const directive = (chatBus, $mdMedia) => {
                return new ToggleChat(chatBus, $mdMedia);
            };

            directive['$inject'] = ['chatBus', '$mdMedia'];

            return directive;
        }
    }

    angular
        .module("app.chat")
        .directive("toggleChat", ToggleChat.factory());
}