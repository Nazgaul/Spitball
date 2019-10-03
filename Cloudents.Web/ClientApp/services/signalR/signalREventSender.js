import { getMainConnection } from './signalrEventService'
import { Notification, NotifyServer } from './signalrEventService'

export const signalRSender = {
    send: function(message, data){
        let mainConnection = getMainConnection();
        NotifyServer(mainConnection, message, data);
    }
}

export const sendEventList = {
    // question:{
    //         addViewr: function(question){
    //             let questionEvent = {
    //                 type: "question",
    //                 action: "addviewr",
    //                 data: question
    //             }
    //             let notificationObj = new Notification(questionEvent);
    //             signalRSender.send("Message", notificationObj);
    //         },
    //         removeViewer: function(question){
    //             let questionEvent = {
    //                 type: "question",
    //                 action: "removeviewer",
    //                 data: question
    //             }
    //             let notificationObj = new Notification(questionEvent);
    //             signalRSender.send("Message", notificationObj);
    //         }
    // }
}