import { connectivityModule } from '../connectivity.module'
import { signlaREvents } from './signalREventHandler'

// do not remove this!
let signalRConnectionPool = [];


function Notification(name, eventObj){
    this.name = `NT_ ${name}`;
    this.extraData = eventObj.extraData;
}


function messageHandler(message){
    //Todo create Notification Object
    /*let notification = new Notification("test", message)*/
    console.log(message);

    //Todo fire signlaREvents correct event
    /*signlaREvents[notification.name]();*/
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
export default function init(connString = '/questionHub'){
    //create a signalR Connection
    let connection = createConnection(connString)
    
    //open the connection and register the events
    startConnection(connection, "ReceiveMessage"); 
}

export function getMainConnection(){
    return signalRConnectionPool[0];
}

 