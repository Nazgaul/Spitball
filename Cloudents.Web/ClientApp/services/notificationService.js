
function Notification(notificationObj){
    this.type = notificationObj.type;
    this.headline = notificationObj.headline;
    this.title = notificationObj.title;
    this.icon = `sbf-${this.type}-notification`;
    this.action = `${notificationObj.action}`;
    this.timeago = notificationObj.timeago;
    this.isVisited = false;
    this.id = generateId();
}



function generateId(){
    return 'xxxx-xxx-xxxxx-xxx-xx-xxxxx'.replace(/[x]/g, function(){
        return (Math.random() * 9) | 0;
    });
}

export default {
       createNotificationObj:(notificationObj)=>{
        return new Notification(notificationObj);
    },
}