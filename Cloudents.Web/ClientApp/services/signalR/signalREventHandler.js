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
        action: function(arrEventObj){
            let questionActions = {
                markAsCorrect: function(dataObj){
                    //feed Object
                    store.dispatch("updateQuestionCorrect", dataObj)
                    //question Object
                    store.dispatch("updateQuestionItemCorrect", dataObj)
                }
            };  
            arrEventObj.forEach((action)=>{
                if(!questionActions[action.type]){
                    console.error(`Action type ${action.type} was not defined in questionActions`)
                }
                questionActions[action.type](action.data)
            })          
        },
        addviewr: function(question){
            store.dispatch("addQuestionViewer", question);
        },
        removeviewer: function(question){
            store.dispatch("removeQuestionViewer", question);
        },
    },
    answer:{
        add: function(arrEventObj){
            arrEventObj.forEach((addedAnswerObj)=>{
                //question Object
                store.dispatch("answerAdded", addedAnswerObj);

                //update answers Number in the main feed
                let actionObj = {
                    questionId: addedAnswerObj.questionId,
                    addCounter: true
                }
                store.dispatch('updateQuestionCounter', actionObj);
            })
        },
        delete: function(arrEventObj){
            arrEventObj.forEach((removedAnswerObj)=>{
                //question Object
                store.dispatch("answerRemoved", removedAnswerObj);
                
                 //update answers Number in the main feed
                 let actionObj = {
                    questionId: removedAnswerObj.questionId,
                    addCounter: false
                }
                store.dispatch('updateQuestionCounter', actionObj);
            })
        }
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