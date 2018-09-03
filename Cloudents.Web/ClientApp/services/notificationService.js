
function Notification(notificationObj){
    this.type = notificationObj.type;
    this.message = notificationObj.message;
    this.icon = `sbf-${this.type}`;
    this.action = `${notificationObj.action}`;
    this.time = notificationObj.timeStamp
}

export default {
       createNotificationObj:(notificationObj)=>{
        return new Notification(notificationObj);
    }

}