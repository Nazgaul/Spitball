'use strict';
(function () {
    angular.module('app.chat').factory('realtimeFactotry', realtimeFactotry);
    //account.$inject = ['$stateParams', '$state', 'userData'];
    realtimeFactotry.$inject = ['Hub', '$rootScope'];
    function realtimeFactotry(Hub, $rootScope) {
        var hub = new Hub('spitballHub', {
            rootPath: (window.dChat || 'https://connect.spitball.co/') + '/s',
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
                console.error(error);
            },

            methods: ['send'],
            stateChanged: function (state) {
                switch (state.newState) {
                    case $.signalR.connectionState.connecting:
                        console.log('connecting');
                        break;
                    case $.signalR.connectionState.connected:
                        console.log('connected');
                        break;
                    case $.signalR.connectionState.reconnecting:
                        console.log('reconnecting');
                        logError('signalr', 'reconnecting');
                        break;
                    case $.signalR.connectionState.disconnected:
                        console.log('disconnected');
                        //hub.connection.start();
                        logError('signalr', 'disconnected');
                        break;
                }
            }
        });

        hub.connection.disconnected(function () {
            setTimeout(function () {
                hub.connection.start();
                //$.connection.hub.start();
            }, 500

            ); // Restart connection after 5 seconds.
        });
        var send = function (userId, message, conversationId, blob) {
            hub.send(userId, message, conversationId, blob);
        };

        return {
            sendMsg: send
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


