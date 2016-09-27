var app;
(function (app) {
    "use strict";
    var States;
    (function (States) {
        States[States["UserList"] = 1] = "UserList";
        States[States["Chat"] = 3] = "Chat";
    })(States || (States = {}));
    var ConnectionStatuses;
    (function (ConnectionStatuses) {
        ConnectionStatuses[ConnectionStatuses["Connected"] = 1] = "Connected";
        ConnectionStatuses[ConnectionStatuses["Disconnected"] = 0] = "Disconnected";
    })(ConnectionStatuses || (ConnectionStatuses = {}));
    var timeoutvalidate;
    var ChatController = (function () {
        function ChatController($scope, $timeout, $stateParams) {
            var _this = this;
            this.$scope = $scope;
            this.$timeout = $timeout;
            this.$stateParams = $stateParams;
            this.state = States.UserList;
            this.connected = false;
            this.scrollSetting = {
                scrollbarPosition: "outside",
                scrollInertia: 50
            };
            $scope.$on("connection-state", function (e, args) {
                if (args.status === ConnectionStatuses.Disconnected) {
                    timeoutvalidate = _this.$timeout(function () {
                        _this.connected = false;
                        $scope.$applyAsync();
                    }, 50);
                }
                else {
                    if (timeoutvalidate) {
                        _this.$timeout.cancel(timeoutvalidate);
                    }
                    _this.connected = true;
                    $scope.$applyAsync();
                }
            });
            if ($stateParams["conversationData"]) {
                this.state = States.Chat;
                this.$scope.$broadcast("go-chat", $stateParams["conversationData"]);
            }
            $scope.$on("open-chat-user", function (e, args) {
                _this.state = States.Chat;
                _this.$scope.$broadcast("go-chat", args);
            });
            $scope.$on("go-chat", function (e, args) {
                _this.state = States.Chat;
                _this.$timeout(function () {
                    _this.$scope.$broadcast("go-conversation", args);
                });
            });
        }
        ChatController.prototype.backFromChat = function () {
            this.state = States.UserList;
        };
        ChatController.$inject = ["$scope", "$timeout", "$stateParams"];
        return ChatController;
    }());
    angular.module("app.chat").controller("ChatController", ChatController);
})(app || (app = {}));
