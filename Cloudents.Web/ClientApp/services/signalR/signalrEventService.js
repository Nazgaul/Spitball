import { connectivityModule } from '../connectivity.module'
import { signalREventHandler } from './signalREventHandler'

const SignlaREventService = {
    "NT_QUESTION_ADDED": signalREventHandler.questions.add,
}

function Notification(name, eventObj){
    this.name = `NT_ ${name}`;
    this.extraData = eventObj.extraData;
}

function registerEvents(connection){
   return connectivityModule.signalR.on(connection)
}

function init(connString){
    const mainConnection = connectivityModule.signalR.createConnection(connString)
    connectivityModule.signalR.start([mainConnection]);
    let connection = registerEvents(mainConnection);
    connection.on("message", function(name, message){
        let notificationObj = new Notification(name, message);
        SignlaREventService[notificationObj.name](notificationObj)
    })
}

init('/connString');