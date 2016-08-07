'use strict';
(function () {
    angular.module('app.chat').factory('realtimeFactotry', realtimeFactotry);
    //account.$inject = ['$stateParams', '$state', 'userData'];
    realtimeFactotry.$inject = ['Hub', '$rootScope'];
    function realtimeFactotry(Hub, $rootScope) {
        var self = this;
        var commands = [];
        var canSend = false;
        var hub = new Hub('spitballHub', {
            rootPath: (window.dChat || 'https://connect.spitball.co') + '/s',
            listeners: {
                chat: function (message, chatRoom, userId, blob) {
                    $rootScope.$broadcast('hub-chat',
                        {
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
                    console.log(userId);
                },
                offline: function (userId) {
                    $rootScope.$broadcast('hub-status', {
                        userId: userId,
                        online: false
                    });
                },
                updateImage: function (blob) {
                    $rootScope.$broadcast('preview-ready', blob);
                }
            },
            errorHandler: function (error) {
                logError('signalr', 'errorHandler', error);
                //console.error(error);
            },

            methods: ['send', 'changeUniversity', 'enterBox', 'leaveBox'],
            stateChanged: function (state) {
                switch (state.newState) {
                    case $.signalR.connectionState.connecting:
                        canSend = false;
                        //console.log('connecting');
                        $rootScope.$broadcast('connection-state', {
                            status: 0
                        });
                        break;
                    case $.signalR.connectionState.connected:
                        canSend = true;
                        for (var i = 0; i < commands.length; i++) {
                            commands[i]();
                        }
                        commands = [];
                        //console.log('connected');
                        $rootScope.$broadcast('connection-state', {
                            status: 1
                        });
                        break;
                    case $.signalR.connectionState.reconnecting:
                        canSend = false;
                        //console.log('reconnecting');
                        logError('signalr', 'reconnecting');
                        $rootScope.$broadcast('connection-state', {
                            status: 0
                        });
                        break;
                    case $.signalR.connectionState.disconnected:
                        canSend = false;
                        //console.log('disconnected');
                        logError('signalr', 'disconnected');
                        $rootScope.$broadcast('connection-state', {
                            status: 0
                        });
                        break;
                }
            }
        });

        hub.connection.disconnected(function () {
            setTimeout(function () {
                hub.connection.start();
            }, 5000); // Restart connection after 5 seconds.
        });
        var send = function (userId, message, conversationId, blob) {
            hub.send(userId, message, conversationId, blob);
        };
        var changeUniversity = function () {
            hub.changeUniversity();
        };

        var enterBox = function (boxId) {
            //if (!boxId) {
            //    return;
            //}
            //if (canSend) {
            //   // hub.invoke('enterBox', [boxId]);
            //    hub.enterBox(boxId);
            //} else {
            //    commands.push(function() {
            //        enterBox.apply(this, [boxId]);
            //    });
            //}
        }
        //function test(boxId) {
        //    commands.push(enterBox.apply(this, [boxId]));
        //}
        var leaveBox = function (boxId) {
            if (!boxId) {
                return;
            }
            hub.leaveBox(boxId);
        }

        return {
            sendMsg: send,
            changeUniversity: changeUniversity,
            enterBox: enterBox,
            leaveBox: leaveBox
        };


        function logError(url, data, payload) {
            var log = {
                data: data,
                payload: payload
            };

            $.ajax({
                type: 'POST',
                url: '/error/jslog/',
                contentType: 'application/json',
                data: angular.toJson({
                    errorUrl: url,
                    errorMessage: JSON.stringify(log),
                    cause: 'ajaxRequest',
                    stackTrace: ''
                })
            });
        }

    }
})();


