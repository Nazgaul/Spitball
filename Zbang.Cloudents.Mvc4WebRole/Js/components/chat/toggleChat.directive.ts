module app {
    "use strict";
    class ToggleChat implements angular.IDirective {
        restrict = "A";

        constructor(private chatBus: IChatBus, private $mdMedia: angular.material.IMedia,
            private userDetailsFactory: IUserDetailsFactory,
            private $rootScope: angular.IRootScopeService,
            private $state: angular.ui.IStateService) {
        }
        link = (scope: IChatTimeAgo, element: ng.IAugmentedJQuery) => {

            var $html = $("html");
            const className = "expanded-chat";
            const hide = "hide";
            if (!this.userDetailsFactory.getUniversity()) {
                element.addClass(hide);
            }

            this.$rootScope.$on("change-university",
                () => {
                    if (this.userDetailsFactory.getUniversity()) {
                        element.removeClass(hide);
                    }
                });
            element.on("click",
                () => {
                    if (this.$mdMedia("xs")) {
                        if (this.$state.current.name === "chat") {
                            scope["app"].back("/dashboard/");
                            return;
                        }
                        this.$state.go("chat");
                    } else {
                        $html.toggleClass(className);
                    }
                });

            scope.$on("expandChat", () => {
                $html.addClass(className);
            });
            if (this.$mdMedia("gt-sm")) {
                return;
            }
            scope.$on("$stateChangeSuccess", (event: angular.IAngularEvent, toState: angular.ui.IState
               /* toParams: ISpitballStateParamsService*/) => {
                if (toState.name === "classChoose") {
                    element.addClass(hide);
                } else {
                    element.removeClass(hide);
                }
            });
            this.chatBus.getUnreadFromServer();
            var counterElem = $(".chat-counter");
            
            var cleanUpFunc = scope.$watch(this.chatBus.getUnread,
                (value: number) => {
                    if (value > 0) {
                        counterElem.text(value.toString()).show();
                    } else {
                        counterElem.hide();
                    }
                });

            var hubChatListener = scope.$on("hub-chat", () => {
               
                var unread = this.chatBus.getUnread();
                this.chatBus.setUnread(++unread);
                scope.$applyAsync();
            });
            scope.$on("$destroy",
                () => {
                    cleanUpFunc();
                    hubChatListener();
                });

        };


        static factory(): angular.IDirectiveFactory {
            const directive = (chatBus: IChatBus, $mdMedia: angular.material.IMedia, userDetailsFactory, $rootScope, $state) => {
                return new ToggleChat(chatBus, $mdMedia, userDetailsFactory, $rootScope, $state);
            };

            directive["$inject"] = ["chatBus", "$mdMedia", "userDetailsFactory", "$rootScope", "$state"];

            return directive;
        }
    }

    angular
        .module("app.chat")
        .directive("toggleChat", ToggleChat.factory());
}