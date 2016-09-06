module app {
    class ToggleChat implements angular.IDirective {
        restrict = 'A';

        constructor(private chatBus, private $mdMedia: angular.material.IMedia,
            private userDetailsFactory: IUserDetailsFactory,
            private $rootScope: angular.IRootScopeService,
            private $state: angular.ui.IStateService) {
        }
        link = (scope: IChatTimeAgo, element: ng.IAugmentedJQuery) => {
            var $html = $('html');
            const className = 'expanded-chat';
            if (!this.userDetailsFactory.getUniversity()) {
                element.addClass('hidden');
            }

            this.$rootScope.$on('change-university',
                () => {
                    if (this.userDetailsFactory.getUniversity()) {
                        element.removeClass('hidden');
                    }
                });
            element.on('click',
                () => {
                    if (this.$mdMedia('xs')) {
                        if (this.$state.current.name === "chat") {
                            scope["app"].back("/dashboard/");
                            return;
                        }
                        this.$state.go("chat");
                        return;
                    }
                    $html.toggleClass(className);

                });
            scope.$on('expandChat', () => {
                $html.addClass(className);
            });
            if (this.$mdMedia('gt-sm')) {
                return;
            }
            var counterElem = $('.chat-counter');
            var cleanUpFunc = scope.$watch(this.chatBus.getUnread,
                (value) => {
                    if (value > 0) {
                        counterElem.text(value.toString()).show();
                    } else {
                        counterElem.hide();
                    }
                });
            scope.$on("$destroy",
                () => {
                    cleanUpFunc();
                });

        };


        public static factory(): angular.IDirectiveFactory {
            const directive = (chatBus, $mdMedia, userDetailsFactory, $rootScope, $state) => {
                return new ToggleChat(chatBus, $mdMedia, userDetailsFactory, $rootScope, $state);
            };

            directive['$inject'] = ['chatBus', '$mdMedia', 'userDetailsFactory', '$rootScope',"$state"];

            return directive;
        }
    }

    angular
        .module("app.chat")
        .directive("toggleChat", ToggleChat.factory());
}