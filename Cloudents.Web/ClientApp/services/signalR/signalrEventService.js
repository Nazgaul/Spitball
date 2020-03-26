import { connectivityModule } from '../connectivity.module'
import { signlaREvents } from './signalREventHandler'
import store from '../../store/index'

function ConnectionObj(objInit){
    this.connection = objInit.connection;
    this.isConnected = objInit.isConnected || false;
    this.connectionQue = objInit.connectionQue || [];
    this.connectionStartCount = objInit.connectionStartCount || 0;
}

// do not remove this!
let signalRConnectionPool = [];

//Signal R Objects ------------------------ start
export function Notification(eventObj) {
    let type = eventObj.type.toLowerCase();
    let action = eventObj.action.toLowerCase();
    this.type = type;
    this.data = eventObj.data;
    this.action = action;
}

function ConnectionQue(connection, message, data) {
    this.connection = connection;
    this.message = message;
    this.data = data;
}
//Signal R Objects ------------------------ end

function messageHandler(message) {
    //Todo create Notification Object
    let notificationObj = new Notification(message);
    // console.log(message);

    //Todo fire signlaREvents correct event
    if (!signlaREvents[notificationObj.type]) {
        console.error(`Event Type - ${notificationObj.type}, Action - ${notificationObj.action} is not exist `);
        return;
    }
    signlaREvents[notificationObj.type][notificationObj.action](notificationObj.data);
}

function connectionOn(connection, message, callback) {
    connectivityModule.sr.on(connection, message, callback);
}

function startConnection(connectionInstance, messageString) {
    connectionInstance.connection.start().then(function () {
        //connection ready register the main Events
        store.dispatch('setIsSignalRConnected', true);
        connectionOn(connectionInstance.connection, messageString, messageHandler);
        connectionOn(connectionInstance.connection, "studyRoomToken", (jwtToken) => {
            store.dispatch('updateJwtToken',jwtToken);
        });
        console.log("signal-R Conected", connectionInstance);
        connectionInstance.isConnected = true;

        //if we have events that cought in the que, then shift them one by one
        if (connectionInstance.connectionQue.length > 0) {
            while (connectionInstance.connectionQue.length) {
                let que = connectionInstance.connectionQue.shift();
                NotifyServer(que.connection, que.message, que.data);
            }
        }
    });
}

function createConnection(connString) {
    let connection = connectivityModule.sr.createConnection(connString);
    let connectionInstance = new ConnectionObj({connection});
    signalRConnectionPool.push(connectionInstance);
    return connectionInstance;
}

async function start(connectionInstance) {
    try {
        await connectionInstance.connection.start();
        store.dispatch('setIsSignalRConnected', true);
        connectionInstance.connectionStartCount = 0;
        connectionInstance.isConnected = true;
        console.log("signal-R Reconected", connectionInstance);

        //if we have events that cought in the que, then shift them one by one
        if(connectionInstance.connectionQue.length > 0){
            while(connectionInstance.connectionQue.length){
                let que = connectionInstance.connectionQue.shift();
                NotifyServer(que.connection, que.message, que.data);
            }
        }
    } catch (err) {
        connectionInstance.connectionStartCount++;
        console.log(err);
        setTimeout(() => {
            if(!connectionInstance.isConnected){
                start(connectionInstance);
            }
        }, 5000 * (connectionInstance.connectionStartCount + 1));
    }
}

//init function is launched from the main.js
export default function init(connString = '/sbHub') {
    //create a signalR Connection
    let connectionInstance = createConnection(connString);


    //reconnect in case connection closes for some reason
    connectionInstance.connection.onclose(async () => {
        let isNotDeleted = signalRConnectionPool.filter((conn)=>{
            return conn.connection.connection.baseUrl === connectionInstance.connection.connection.baseUrl;
        });
        if(isNotDeleted.length > 0){
            store.dispatch('setIsSignalRConnected', false);
            connectionInstance.isConnected = false;
            await start(connectionInstance);
        }
    });

    //open the connection and register the events
    startConnection(connectionInstance, "Message");
}

export function CloseConnection(connString){
    let connectionInstance = null;
    let indexOfInstance = -1;
    signalRConnectionPool.forEach((con, index)=>{
        if(con.connection.connection.baseUrl.indexOf(connString) > -1){
            connectionInstance = con;
            indexOfInstance = index;
        }
    });
    signalRConnectionPool.splice(indexOfInstance, 1);
    if(!!connectionInstance){
        connectionInstance.connection.stop();
    }
}

export function NotifyServer(connection, message, data) {
    let mainConnectionInstance = signalRConnectionPool[0];
    let srConnection = connection.connection;
    if (mainConnectionInstance.isConnected) {
        return connectivityModule.sr.invoke(srConnection, message, data);
    } else {
        mainConnectionInstance.connectionQue.push(new ConnectionQue(srConnection, message, data));
    }
}

export function getMainConnection() {
    return signalRConnectionPool[0];
}

