import signalRInit from '../services/signalR/init';
import { SignalR } from '../services/Dto/signalR';
import { signlaREvents } from '../services/signalR/signalREventHandler';

export default ({hubPath}) => {
  return store => {
    const connection = signalRInit(hubPath);
    connection.on('Message', (data) => {
      let event = new SignalR.Notification(data);
      if(event.type === 'chat') {
        if(event.action === 'add') {
          store.dispatch("signalRAddMessage", event.data[0]);
        } else if(event.action === 'update') {
          store.dispatch("checkUnreadMessageFromSignalR", event.data[0]);
        }
      } else {
        signlaREvents[event.type][event.action](event.data);
      }
    });

    // signalR Reconnecting
    connection.onreconnecting(() => {
      store.dispatch('setIsSignalRConnected', false);
    });
    connection.onreconnected(() => {
      store.dispatch('setIsSignalRConnected', true);
    });

    store.subscribe((mutation) => {
      switch(mutation.type) {
        case 'updateUser':
          if(connection.state === 'Disconnected') {
            connection.start().then(() => {
              store.dispatch('setIsSignalRConnected', true);  
            });
          }
          break;
        case 'signalR_emit':
          connection.invoke(mutation.payload.message, mutation.payload.data).then(res => {
            console.log(res);
          })
          break;
        case 'signalR_reconnect':
          connection.stopConnection().then(() => { connection.start() });
          break;
        case 'signalR_disconnect':

          break;
        default:
          return;
      }
    })
  }
}