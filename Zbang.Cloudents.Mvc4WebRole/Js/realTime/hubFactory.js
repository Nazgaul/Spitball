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



$.getScript('https://develop-connect.spitball.co/s/signalr/hubs', function () {

    var chat = $.connection.chatHub;
    // Create a function that the hub can call to broadcast messages.
    chat.client.broadcastMessage = function (name, message) {
        // Html encode display name and message. 
        console.log(name, message);
    };
    $.connection.hub.url = 'https://develop-connect.spitball.co/s';
    //var myHub = $.connection.crossDomainHub;
    //
    //myHub.client.showTime = function (dateTimeFromServer) {
    //    $('#dateTime').html(dateTimeFromServer);
    //}

    $.connection.hub.start().done(function () {
        window.setInterval(function () {
            chat.server.send('ram', 'yaari');
        }, 2000);
    });
});

