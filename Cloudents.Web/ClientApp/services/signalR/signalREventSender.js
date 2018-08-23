import { getMainConnection } from './signalrEventService'
import { connectivityModule } from '../connectivity.module'
import { Notification } from './signalrEventService'


function notifyServer(connection, message, data){
    return connectivityModule.sr.invoke(connection, message, data)
 }

export const signalRSender = {
    send: function(message, data){
        let mainConnection = getMainConnection();
        notifyServer(mainConnection, message, data);
    }
}


export const sendEventList = {
    question:{
            addViewr: function(question){
                let questionEvent = {
                    type: "question",
                    action: "addviewr",
                    data: question
                }
                let notificationObj = new Notification(questionEvent);
                signalRSender.send("Message", notificationObj);
            },
            removeViewer: function(question){
                let questionEvent = {
                    type: "question",
                    action: "removeviewr",
                    data: question
                }
                let notificationObj = new Notification(questionEvent);
                signalRSender.send("Message", notificationObj);
            }
    }
}