import { getMainConnection } from './signalrEventService'
import { connectivityModule } from '../connectivity.module'


function notifyServer(connection, message, data){
    return connectivityModule.sr.invoke(connection, message, data)
 }

export const signalRSender = {
    send: function(message, data){
        let mainConnection = getMainConnection();
        notifyServer(mainConnection, message, data);
    }
}