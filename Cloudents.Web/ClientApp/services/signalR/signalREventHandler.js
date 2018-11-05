import store from '../../store/index'

export const signlaREvents = {
    question: {
        add: function(arrEventObj){
            arrEventObj.forEach((questionToAdd)=>{
                store.dispatch("addQuestionItemAction", questionToAdd);
            })
        },
        delete: function(arrEventObj){
            arrEventObj.forEach((questionToRemove)=>{
                store.dispatch("removeQuestionItemAction", questionToRemove);
            })
        },
        update: function(arrEventObj){
            arrEventObj.forEach((question)=>{
                store.dispatch("updateQuestionItem", question)
                store.dispatch("updateQuestionSignalR", question)
            })
        },
        addviewr: function(question){
            store.dispatch("addQuestionViewer", question);
        },
        removeviewer: function(question){
            store.dispatch("removeQuestionViewer", question);
        },
    },
    notification: {
        add: function(arrEventObj){
            arrEventObj.forEach((notificationToAdd)=>{
                store.dispatch("addNotificationItemAction", notificationToAdd);
            })
        },
        delete: function(arrEventObj){
            arrEventObj.forEach((notificationToRemove)=>{
                store.dispatch("removeNotification", notificationToRemove);
            })
        },
        update: function(arrEventObj){
            arrEventObj.forEach((notificationToUpdate)=>{
                store.dispatch("updateNotification", notificationToUpdate)
            })
        },
    }
};