var app;
(function (app) {
    "use scrict";
    var Guid = (function () {
        function Guid() {
        }
        Guid.newGuid = function () {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        };
        return Guid;
    }());
    app.Guid = Guid;
    var RealTimeFactory = (function () {
        function RealTimeFactory(Hub, $rootScope, ajaxService) {
            var _this = this;
            this.Hub = Hub;
            this.$rootScope = $rootScope;
            this.ajaxService = ajaxService;
            this.commands = [];
            this.boxIds = [];
            this.assingBoxes = function (boxIds) {
                if (!boxIds) {
                    return;
                }
                if (!angular.isArray(boxIds)) {
                    boxIds = [boxIds];
                    _this.boxIds = _this.boxIds.concat(boxIds);
                }
                if (_this.canSend) {
                    _this.hub.invoke('enterBoxes', boxIds);
                }
                else {
                    _this.commands.push(function () {
                        _this.assingBoxes.apply(_this, [boxIds]);
                    });
                }
            };
            this.hub = new Hub('spitballHub', {
                rootPath: (dChat || 'https://connect.spitball.co') + '/s',
                listeners: {
                    chat: function (message, chatRoom, userId, blob) {
                        $rootScope.$broadcast("hub-chat", {
                            message: message,
                            chatRoom: chatRoom,
                            userId: userId,
                            blob: blob
                        });
                    },
                    online: function (userId) {
                        $rootScope.$broadcast('hub-status', {
                            userId: userId,
                            online: true
                        });
                    },
                    offline: function (userId) {
                        $rootScope.$broadcast('hub-status', {
                            userId: userId,
                            online: false
                        });
                    },
                    updateImage: function (blob) {
                        $rootScope.$broadcast('preview-ready', blob);
                    },
                    updateThumbnail: function (itemId) {
                        $rootScope.$broadcast('update-thumbnail', itemId);
                    },
                    echo: function (i) {
                    }
                },
                errorHandler: function (error) {
                    ajaxService.logError('signalr', 'errorHandler', error);
                },
                methods: ['send', 'changeUniversity', 'enterBoxes'],
                stateChanged: function (state) {
                    switch (state.newState) {
                        case $.signalR.connectionState.connecting:
                            _this.canSend = false;
                            $rootScope.$broadcast('connection-state', {
                                status: 0
                            });
                            break;
                        case $.signalR.connectionState.connected:
                            _this.canSend = true;
                            for (var i = 0; i < _this.commands.length; i++) {
                                _this.commands[i]();
                            }
                            _this.commands = [];
                            $rootScope.$broadcast('connection-state', {
                                status: 1
                            });
                            break;
                        case $.signalR.connectionState.reconnecting:
                            _this.canSend = false;
                            ajaxService.logError('signalr', 'reconnecting');
                            $rootScope.$broadcast('connection-state', {
                                status: 0
                            });
                            break;
                        case $.signalR.connectionState.disconnected:
                            _this.canSend = false;
                            $rootScope.$broadcast('connection-state', {
                                status: 0
                            });
                            break;
                    }
                }
            });
            $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams) {
                if (toParams.boxId) {
                    if (_this.boxIds.indexOf(toParams.boxId) === -1) {
                        _this.assingBoxes(toParams.boxId);
                    }
                }
            });
            this.hub.connection.disconnected(function () {
                setTimeout(function () {
                    _this.hub.connection.start();
                }, 5000);
            });
        }
        RealTimeFactory.prototype.sendMsg = function (userId, message, conversationId, blob) {
            this.hub.invoke('send', userId, message, conversationId, blob);
        };
        RealTimeFactory.prototype.changeUniversity = function () {
            this.hub.invoke('changeUniversity');
        };
        ;
        RealTimeFactory.$inject = ["Hub", "$rootScope", "ajaxService2"];
        return RealTimeFactory;
    }());
    angular.module('app.chat').service('realtimeFactory', RealTimeFactory);
})(app || (app = {}));
//# sourceMappingURL=hubFactory.js.map