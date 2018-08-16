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
   return connectivityModule.sr.on(connection)
}

export default function init(connString = '/questionHub'){
    const mainConnection = connectivityModule.sr.createConnection(connString)
    connectivityModule.sr.start([mainConnection]);
    let connectionOn = registerEvents(mainConnection);
    connectionOn("ReceiveMessage", function (message) {
        console.log(message);
        //let notificationObj = new Notification(message);
        //SignlaREventService[notificationObj.name](notificationObj)
    })
}

 