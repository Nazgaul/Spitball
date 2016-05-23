'use strict';
(function () {
    angular.module('app.chat').factory('realtimeFactotry', realtimeFactotry);
    //account.$inject = ['$stateParams', '$state', 'userData'];
    realtimeFactotry.$inject = ['Hub', '$rootScope'];
    function realtimeFactotry(Hub, $rootScope) {
        var hub = new Hub('spitballHub', {
            rootPath: window.dChat + '/s',
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
                    console.log('online', userId);
                    $rootScope.$broadcast('hub-status', {
                        userId: userId,
                        online: true
                    });
                },
                offline: function (userId) {
                    console.log('offline', userId);
                    $rootScope.$broadcast('hub-status', {
                        userId: userId,
                        online: false
                    });
                    
                }
                //chatRoomId: function (message) {
                //    $rootScope.$broadcast('hub-chat-roomid', { message: message });
                //},
                //chatRoom: function (id, user) {
                //    $rootScope.$broadcast('hub-chat-room', { id: id, user: user });
                //}
            },
            errorHandler: function (error) {
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
                        break;
                    case $.signalR.connectionState.disconnected:
                        console.log('disconnected');
                        break;
                }
            }
        });
        var send = function (userId, message, conversationId, blob) {
            hub.send(userId, message, conversationId, blob);
        };

        return {
            sendMsg: send
        };

    }
})();


