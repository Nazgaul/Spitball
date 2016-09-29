module app {
    "use strict";
    enum States {
        UserList = 1,
        Chat = 3
    }
    //enum ConnectionStatuses {
    //    Connected = 1,
    //    Disconnected = 0
    //}
    var timeoutvalidate:angular.IPromise<void>;
    class ChatController {
        static $inject = ["$scope", "$timeout", "$stateParams","realtimeFactory"];
        state = States.UserList;
        connected = false;
        constructor(
            private $scope: angular.IScope,
            private $timeout: angular.ITimeoutService,
            private $stateParams: angular.ui.IStateParamsService,
            private realtimeFactory: IRealtimeFactory) {
            timeoutvalidate = null;
            $scope.$watch(realtimeFactory.isConnected,
                (newValue: boolean, oldValue: boolean) => {
                    //console.log(newValue, oldValue);
                    if (newValue === false) {
                        // firefox issue upon reload
                        timeoutvalidate = this.$timeout(() => {
                            this.connected = false;
                            $scope.$applyAsync();
                        }, 50);
                    } else {
                        if (timeoutvalidate) {
                            this.$timeout.cancel(timeoutvalidate);
                        }
                        this.connected = true;
                        $scope.$applyAsync();
                    }
                });
            //$scope.$on("connection-state", (e:angular.IAngularEvent, args:any) => {
            //    if (args.status === ConnectionStatuses.Disconnected) {
            //        // firefox issue upon reload
            //        timeoutvalidate = this.$timeout(() => {
            //            this.connected = false;
            //            $scope.$applyAsync();
            //        },50);
            //    } else {
            //        if (timeoutvalidate) {
            //            this.$timeout.cancel(timeoutvalidate);
            //        }
            //        this.connected = true;
            //        $scope.$applyAsync();
            //    }
            //});

            if ($stateParams["conversationData"]) {
                this.state = States.Chat;
                this.$scope.$broadcast("go-chat", $stateParams["conversationData"]);

            }
            $scope.$on("open-chat-user", (e: angular.IAngularEvent, args:any) => {
                this.state = States.Chat;
                this.$scope.$broadcast("go-chat", args);
            });
            $scope.$on("go-chat",
                (e: angular.IAngularEvent, args:any) => {
                    this.state = States.Chat;
                    this.$timeout(() => {
                        this.$scope.$broadcast("go-conversation", args);
                    });
                });
        }
        // TODO: get rid of
        scrollSetting = {
            scrollbarPosition: "outside",
            scrollInertia: 50
        };
        backFromChat() {
            this.state = States.UserList;
        }

    }

    angular.module("app.chat").controller("ChatController", ChatController);
}


