import { connectivityModule } from '../connectivity.module'
import { signlaREvents } from './signalREventHandler'
import store from '../../store/index'

// do not remove this!
let signalRConnectionPool = [];

let connectionState = {
    isConnected:false,
    connectionQue: []
}


//Signal R Objects ------------------------ start
export function Notification(eventObj){
    let type = eventObj.type.toLowerCase();
    let action = eventObj.action.toLowerCase();
    this.type = type;
    this.data = eventObj.data;
    this.action = action;
}

function ConnectionQue(connection, message, data){
    this.connection = connection;
    this.message = message;
    this.data = data;
}
//Signal R Objects ------------------------ end

function messageHandler(message){
    //Todo create Notification Object
    let notificationObj = new Notification(message)
    console.log(message);

    //Todo fire signlaREvents correct event
    if(!signlaREvents[notificationObj.type]){
        console.error(`Event Type - ${notificationObj.type}, Action - ${notificationObj.action} is not exist `);
        return;
    }
    signlaREvents[notificationObj.type][notificationObj.action](notificationObj.data);
}

function connectionOn(connection, message, callback){
    connectivityModule.sr.on(connection, message, callback)
 }

function startConnection(connection, messageString){
        connection.start().then(function(){
            //connection ready register the main Events
            store.dispatch('setIsSignalRConnected', true);
            connectionOn(connection, messageString, messageHandler);
            console.log("signal-R Conected");
            connectionState.isConnected = true;
            
            //if we have events that cought in the que, then shift them one by one
            if(connectionState.connectionQue.length > 0){
                while(connectionState.connectionQue.length){
                    let que = connectionState.connectionQue.shift()
                    NotifyServer(que.connection, que.message, que.data)
                }
            }
        }); 
}

function createConnection(connString){
    let newConnection = connectivityModule.sr.createConnection(connString);
    signalRConnectionPool.push(newConnection);

    return newConnection;
}

async function start(connection) {
    try {
        await connection.start().then(()=>{
            store.dispatch('setIsSignalRConnected', true);
            connectionState.isConnected = true;
            console.log("signal-R Reconected");

            //if we have events that cought in the que, then shift them one by one
            if(connectionState.connectionQue.length > 0){
                while(connectionState.connectionQue.length){
                    let que = connectionState.connectionQue.shift()
                    NotifyServer(que.connection, que.message, que.data)
                }
            }
        });
    } catch (err) {
        console.log(err);
        setTimeout(() => {
            if(!connectionState.isConnected){
                start(connection);
            }
        }, 5000);
    }
};

//init function is launched from the main.js
export default function init(connString = '/sbHub'){
    //create a signalR Connection
    let connection = createConnection(connString)
    
    //reconnect in case connection closes for some reason
    connection.onclose(async () => {
        store.dispatch('setIsSignalRConnected', false);
        connectionState.isConnected = false;
        await start(connection);
    });

    //open the connection and register the events
    startConnection(connection, "Message"); 
}

export function NotifyServer(connection, message, data){
    if(connectionState.isConnected){
        return connectivityModule.sr.invoke(connection, message, data)
    }else{
        connectionState.connectionQue.push(new ConnectionQue(connection, message, data))
    }
}

export function reconnectSignalR(){
    if(signalRConnectionPool.length > 0){
        signalRConnectionPool.forEach((connection, index)=>{
            connection.stop()
            signalRConnectionPool.splice(index, 1);
        })
    }
    init();
}

export function getMainConnection(){
    return signalRConnectionPool[0];
}

 