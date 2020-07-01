import { SignalR } from '../services/Dto/signalR';
import { signlaREvents } from '../services/signalR/signalREventHandler';

export default ({ hubPath }) => {
  return store => {
    let connection = null
    store.subscribe((mutation) => {
      switch (mutation.type) {
        case 'updateUser':
          import('@microsoft/signalr').then((signalR) => {
            connection = new signalR.HubConnectionBuilder().withUrl(hubPath)
            .configureLogging(signalR.LogLevel.Information)
            .withAutomaticReconnect()
            .build();

            connection.on('Message', (data) => {
              let event = new SignalR.Notification(data);
              if (event.type === 'chat') {
                if (event.action === 'add') {
                  store.dispatch("signalRAddMessage", event.data[0]);
                } else if (event.action === 'update') {
                  store.dispatch("checkUnreadMessageFromSignalR", event.data[0]);
                }
              } else {
                signlaREvents[event.type][event.action](event.data);
              }
            });
            connection.on("studyRoomToken", (jwtToken,studyRoomId) => {
              if (store.getters.getRoomIdSession == studyRoomId) {
                store.dispatch('updateJwtToken', jwtToken);
              }
            });
        
            // signalR Reconnecting
            connection.onreconnecting(() => {
              store.dispatch('setIsSignalRConnected', false);
            });
            connection.onreconnected(() => {
              store.dispatch('setIsSignalRConnected', true);
            });

            if (connection.state === 'Disconnected') {
              connection.start().then(() => {
                store.dispatch('setIsSignalRConnected', true);
              });
            }
        })
          break;
        case 'signalR_emit':
          connection.invoke(mutation.payload.message, mutation.payload.data);
          break;
        case 'signalR_reconnect':
          connection.stopConnection().then(() => { connection.start() });
          break;
        case 'signalR_disconnect':

          break;
        // case 'ROOM_PROPS':
        // //  connection.invoke("addStudyRoomGroup", mutation.roomId);
        //     debugger;
      }
    })
  }
}