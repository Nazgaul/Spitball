export const SignalR = {
    Default:function(objInit){
        this.connection = objInit.connection;
        this.isConnected = objInit.isConnected || false;
        this.connectionQue = objInit.connectionQue || [];
        this.connectionStartCount = objInit.connectionStartCount || 0;
    },
    Notification: function(objInit) {
        this.type = objInit.type.toLowerCase();
        this.action = objInit.action.toLowerCase();
        this.data = objInit.data;
    },
    ConnectionQue: function(objInit) {
        this.connection = objInit.connection;
        this.message = objInit.message;
        this.data = objInit.data;
    }
 }