import { connectivityModule } from '../connectivity.module'
import { signlaREvents } from './signalREventHandler'

// do not remove this!
let signalRConnectionPool = [];


export function Notification(eventObj){
    let type = eventObj.type.toLowerCase();
    let action = eventObj.action.toLowerCase();
    this.type = type;
    this.data = eventObj.data;
    this.action = action;
}


function messageHandler(message){
    //Todo create Notification Object
    let notificationObj = new Notification(message)
    console.log(message);

    //Todo fire signlaREvents correct event
    signlaREvents[notificationObj.type][notificationObj.action](notificationObj.data);
}

function connectionOn(connection, message, callback){
    connectivityModule.sr.on(connection, message, callback)
 }

function startConnection(connection, messageString){
    connection.start().then(function(){
        //connection ready register the main Events
        connectionOn(connection, messageString, messageHandler);
    });
}

function createConnection(connString){
    let newConnection = connectivityModule.sr.createConnection(connString);
    signalRConnectionPool.push(newConnection);

    return newConnection;
}

//init function is launched from the main.js
export default function init(connString = '/sbHub'){
    //create a signalR Connection
    let connection = createConnection(connString)
    
    //open the connection and register the events
    startConnection(connection, "Message"); 
}

export function getMainConnection(){
    return signalRConnectionPool[0];
}

 