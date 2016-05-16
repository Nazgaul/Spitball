//var chat = $.connection.chatHub;
//// Create a function that the hub can call to broadcast messages.
//chat.client.broadcastMessage = function (name, message) {
//    // Html encode display name and message. 
//    console.log(name, message);
//};


//$.connection.hub.url = 'https://develop-connect.spitball.co';

//$.connection.hub.start()
//    .done(function () {
//        console.log('Now connected, connection ID=' + $.connection.hub.id);
//        window.setTimeout(function () {
//            chat.server.send('ram', 'yaari');
//        }, 2000);
//    })
//    .fail(function (e) {
//        console.log('Could not Connect!', e);
//    });



//var connection = $.hubConnection('https://develop-connect.spitball.co', { useDefaultPath: false });
////connection.url = 'https://develop-connect.spitball.co';
//////var contosoChatHubProxy = connection.createHubProxy('contosoChatHub');
//////contosoChatHubProxy.on('addContosoChatMessageToPage', function (name, message) {
//////    console.log(name + ' ' + message);
//////});
//connection.logging = true;
//connection.start().done(function () {
//    console.log('askdfhasdk')
//    // Wire up Send button to call NewContosoChatMessage on the server.
//    //$('#newContosoChatMessage').click(function () {
//    //    contosoChatHubProxy.invoke('newContosoChatMessage', $('#displayname').val(), $('#message').val());
//    //    $('#message').val('').focus();
//    //});
//}).fail(function (e) { console.log('Could not Connect!', e); });

'use strict';
(function() {
    angular.module('app.chat').factory('realtimeFactotry', realtimeFactotry);
    //account.$inject = ['$stateParams', '$state', 'userData'];
    realtimeFactotry.$inject = ['Hub','$rootScope'];
    function realtimeFactotry(Hub, $rootScope) {
        var hub = new Hub('spitballHub', {
            rootPath: 'https://develop-connect.spitball.co/s',
            listeners: {
                send : function(message) {
                   // console.log(message);
                    $rootScope.$broadcast('hub-send', message);
                }
            },
            errorHandler: function (error) {
                console.error(error);
            },

            //specify a non default root
            //rootPath: '/api
            methods: ['send'],
            stateChanged: function (state) {
                console.log(state);
                //switch (state.newState) {
                //    case $.signalR.connectionState.connecting:
                //        //your code here
                //        break;
                //    case $.signalR.connectionState.connected:
                //        //your code here
                //        break;
                //    case $.signalR.connectionState.reconnecting:
                //        //your code here
                //        break;
                //    case $.signalR.connectionState.disconnected:
                //        //your code here
                //        break;
                //}
            }
        });
        var send = function(userId, message) {
            hub.send(userId, message);
        }

        return {
            sendMsg:send
        }

    }
})();

//$.getScript('https://develop-connect.spitball.co/s/signalr/hubs', function () {
//
//    var chat = $.connection.spitballHub;
//    // Create a function that the hub can call to broadcast messages.
//    chat.client.send = function (message) {
//        // Html encode display name and message. 
//        console.log(message);
//    };
//    $.connection.hub.url = 'https://develop-connect.spitball.co/s';
//    //var myHub = $.connection.crossDomainHub;
//    //
//    //myHub.client.showTime = function (dateTimeFromServer) {
//    //    $('#dateTime').html(dateTimeFromServer);
//    //}
//
//    $.connection.hub.start().done(function () {
//        console.log('done')
//
//    });
//    window.si = function (userid) {
//        window.setInterval(function () {
//            chat.server.send(userid, 'hello ram');
//        }, 10000);
//    }
//});

