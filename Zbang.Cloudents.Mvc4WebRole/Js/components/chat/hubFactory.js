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
    //interface ISbHub extends ngSignalr.Hub {
    //    sendMsg(userId: number, message: string, conversationId: Guid, blob: string): void;
    //}
    var RealTimeFactory = (function () {
        function RealTimeFactory(Hub, $rootScope, ajaxService) {
            var _this = this;
            this.Hub = Hub;
            this.$rootScope = $rootScope;
            this.ajaxService = ajaxService;
            this.commands = [];
            this.assingBoxes = function (boxIds) {
                if (!boxIds) {
                    return;
                }
                if (!angular.isArray(boxIds)) {
                    boxIds = [boxIds];
                }
                if (_this.canSend) {
                    // hub.invoke('enterBox', [boxId]);
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
                        $rootScope.$broadcast('hub-chat', {
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
                        //console.log('echo', i);
                    }
                },
                errorHandler: function (error) {
                    ajaxService.logError('signalr', 'errorHandler', error);
                    //console.error(error);
                },
                methods: ['send', 'changeUniversity', 'enterBoxes'],
                stateChanged: function (state) {
                    switch (state.newState) {
                        case $.signalR.connectionState.connecting:
                            _this.canSend = false;
                            //console.log('connecting');
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
                            //console.log('connected');
                            $rootScope.$broadcast('connection-state', {
                                status: 1
                            });
                            break;
                        case $.signalR.connectionState.reconnecting:
                            _this.canSend = false;
                            //console.log('reconnecting');
                            ajaxService.logError('signalr', 'reconnecting');
                            $rootScope.$broadcast('connection-state', {
                                status: 0
                            });
                            break;
                        case $.signalR.connectionState.disconnected:
                            _this.canSend = false;
                            //console.log('disconnected');
                            ajaxService.logError('signalr', 'disconnected');
                            $rootScope.$broadcast('connection-state', {
                                status: 0
                            });
                            break;
                    }
                }
            });
            this.hub.connection.disconnected(function () {
                setTimeout(function () {
                    _this.hub.connection.start();
                }, 5000); // Restart connection after 5 seconds.
            });
        }
        RealTimeFactory.prototype.sendMsg = function (userId, message, conversationId, blob) {
            this.hub.invoke('send', userId, message, conversationId, blob);
        };
        RealTimeFactory.prototype.changeUniversity = function () {
            this.hub.invoke('changeUniversity');
        };
        ;
        RealTimeFactory.$inject = ['Hub', '$rootScope', 'ajaxService2'];
        return RealTimeFactory;
    }());
    angular.module('app.chat').service('realtimeFactotry', RealTimeFactory);
})(app || (app = {}));
//'use strict';
//(function () {
//    angular.module('app.chat').factory('realtimeFactotry', realtimeFactotry);
//    //account.$inject = ['$stateParams', '$state', 'userData'];
//    realtimeFactotry.$inject = ['Hub', '$rootScope', 'ajaxService2'];
//    function realtimeFactotry(Hub, $rootScope, ajaxService2) {
//        // var self = this;
//        var commands = [];
//        var canSend;
//        var hub = new Hub('spitballHub', {
//            rootPath: (window.dChat || 'https://connect.spitball.co') + '/s',
//            listeners: {
//                chat: function (message, chatRoom, userId, blob) {
//                    $rootScope.$broadcast('hub-chat',
//                        {
//                            message: message,
//                            chatRoom: chatRoom,
//                            userId: userId,
//                            blob: blob
//                        });
//                },
//                online: function (userId) {
//                    $rootScope.$broadcast('hub-status', {
//                        userId: userId,
//                        online: true
//                    });
//                    console.log(userId);//
//                },
//                offline: function (userId) {
//                    $rootScope.$broadcast('hub-status', {
//                        userId: userId,
//                        online: false
//                    });
//                },
//                updateImage: function (blob) {
//                    $rootScope.$broadcast('preview-ready', blob);
//                },
//                updateThumbnail: function (itemId) {
//                    $rootScope.$broadcast('update-thumbnail', itemId);
//                },
//                echo: function (i) {
//                    //console.log('echo', i);
//                }
//            },
//            errorHandler: function (error) {
//                logError('signalr', 'errorHandler', error);
//                //console.error(error);
//            },
//            methods: ['send', 'changeUniversity', 'enterBoxes'],
//            stateChanged: function (state) {
//                switch (state.newState) {
//                    case $.signalR.connectionState.connecting:
//                        canSend = false;
//                        //console.log('connecting');
//                        $rootScope.$broadcast('connection-state', {
//                            status: 0
//                        });
//                        break;
//                    case $.signalR.connectionState.connected:
//                        canSend = true;
//                        for (var i = 0; i < commands.length; i++) {
//                            commands[i]();
//                        }
//                        commands = [];
//                        //console.log('connected');
//                        $rootScope.$broadcast('connection-state', {
//                            status: 1
//                        });
//                        break;
//                    case $.signalR.connectionState.reconnecting:
//                        canSend = false;
//                        //console.log('reconnecting');
//                        logError('signalr', 'reconnecting');
//                        $rootScope.$broadcast('connection-state', {
//                            status: 0
//                        });
//                        break;
//                    case $.signalR.connectionState.disconnected:
//                        canSend = false;
//                        //console.log('disconnected');
//                        logError('signalr', 'disconnected');
//                        $rootScope.$broadcast('connection-state', {
//                            status: 0
//                        });
//                        break;
//                }
//            }
//        });
//        hub.connection.disconnected(function () {
//            setTimeout(function () {
//                hub.connection.start();
//            }, 5000); // Restart connection after 5 seconds.
//        });
//        var send = function (userId, message, conversationId, blob) {
//            hub.send(userId, message, conversationId, blob);
//        };
//        var changeUniversity = function () {
//            hub.changeUniversity();
//        };
//        var assingBoxes = function (boxIds) {
//            if (!boxIds) {
//                return;
//            }
//            if (!angular.isArray(boxIds)) {
//                boxIds = [boxIds];
//            }
//            if (canSend) {
//                // hub.invoke('enterBox', [boxId]);
//                hub.enterBoxes(boxIds);
//            } else {
//                commands.push(function () {
//                    assingBoxes.apply(this, [boxIds]);
//                });
//            }
//        }
//        //function test(boxId) {
//        //    commands.push(enterBox.apply(this, [boxId]));
//        //}
//        //var leaveBox = function (boxId) {
//        //    if (!boxId) {
//        //        return;
//        //    }
//        //    hub.leaveBox(boxId);
//        //}
//        return {
//            sendMsg: send,
//            changeUniversity: changeUniversity,
//            assingBoxes: assingBoxes
//            // leaveBox: leaveBox
//        };
//        // todo: duplicate
//        function logError(url, data, payload) {
//            var log = {
//                data: data,
//                payload: payload
//            };
//            $.ajax({
//                type: 'POST',
//                url: '/error/jslog/',
//                contentType: 'application/json',
//                data: angular.toJson({
//                    errorUrl: url,
//                    errorMessage: JSON.stringify(log),
//                    cause: 'ajaxRequest',
//                    stackTrace: ''
//                })
//            });
//        }
//    }
//})();
