var chat = $.connection.chatHub;
// Create a function that the hub can call to broadcast messages.
chat.client.broadcastMessage = function (name, message) {
    // Html encode display name and message. 
    console.log(name, message);
};


$.connection.hub.url = 'https://develop-connect.spitball.co/s/'
$.connection.hub.start().done(function () { console.log('Now connected, connection ID=' + $.connection.hub.id); })
    .fail(function (e) { console.log('Could not Connect!', e); });

window.setTimeout(function () {
    chat.server.send('ram', 'yaari');
}, 2000);