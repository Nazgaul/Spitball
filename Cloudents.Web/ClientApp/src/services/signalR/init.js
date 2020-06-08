import * as signalR from '@microsoft/signalr';

export default (conString) => {

    let connection = new signalR.HubConnectionBuilder().withUrl(conString)
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .build();

    return connection
}