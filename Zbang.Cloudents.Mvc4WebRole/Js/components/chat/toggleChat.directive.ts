module app {
    class ToggleChat implements angular.IDirective {
        restrict = 'A';

        constructor(private chatBus, private $mdMedia: angular.material.IMedia) {
        }
        link = (scope: IChatTimeAgo, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
            var $html = $('html');
            const className = 'expanded-chat';
            angular.element(element).on('click', () => {
                $html.toggleClass(className);

            });
            scope.$on('expandChat', function () {
                $html.addClass(className);
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