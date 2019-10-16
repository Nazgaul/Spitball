import store from '../../store/index'

export const signlaREvents = {
    question: {
        add: function(arrEventObj){
            arrEventObj.forEach((questionToAdd)=>{
                store.dispatch("addQuestionItemAction", questionToAdd);
            });
        },
        delete: function(arrEventObj){
            arrEventObj.forEach((questionToRemove)=>{
                store.dispatch("removeQuestionItemAction", questionToRemove);
            });
        },
        action: function(arrEventObj){
            let questionActions = {
                markascorrect: function(dataObj){
                    //feed Object
                    store.dispatch("updateQuestionCorrect", dataObj);
                    //question Object
                    store.dispatch("updateQuestionItemCorrect", dataObj);
                }
            };  
            arrEventObj.forEach((action)=>{
                if(!questionActions[action.type]){
                    console.error(`Action type ${action.type} was not defined in Question questionActions`);
                }
                questionActions[action.type](action.data);
            });
        }
        
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
                };
                store.dispatch('Feeds_updateCounter', actionObj);
            });
        },
        delete: function(arrEventObj){
            arrEventObj.forEach((removedAnswerObj)=>{
                //question Object
                store.dispatch("answerRemoved", removedAnswerObj);
                
                 //update answers Number in the main feed
                 let actionObj = {
                    questionId: removedAnswerObj.questionId,
                    addCounter: false
                };
                 store.dispatch('Feeds_updateCounter', actionObj);
            });
        }
    },
    user:{
        update:function(arrEventObj){
            arrEventObj.forEach((user)=>{
                
                    store.dispatch('signalR_SetBalance', user.balance);
                
            });

        },
        action: function(arrEventObj){
            let userActions = {
                logout: function(data){
                    store.dispatch("logout", data);
                },
                onlinestatus: function(data){
                    store.dispatch("setUserStatus", data);
                },
                paymentreceived: function(data){
                    store.dispatch("signalR_ReleasePaymeStatus");
                }
            };  
            arrEventObj.forEach((action)=>{
                if(!userActions[action.type]){
                    console.error(`Action type ${action.type} was not defined in User userActions`);
                    return;
                }
                userActions[action.type](action.data);
            });
        },
    },
    notification: {
        add: function(arrEventObj){
            arrEventObj.forEach((notificationToAdd)=>{
                store.dispatch("addNotificationItemAction", notificationToAdd);
            });
        },
        delete: function(arrEventObj){
            arrEventObj.forEach((notificationToRemove)=>{
                store.dispatch("removeNotification", notificationToRemove);
            });
        },
        update: function(arrEventObj){
            arrEventObj.forEach((notificationToUpdate)=>{
                store.dispatch("updateNotification", notificationToUpdate);
            });
        },
    },
    system: {
        action: function(arrEventObj){
            let systemActions = {
                toaster: function(data){
                    let serverData = {
                        text: data.text,
                        timeout: data.timeout || 5000
                    };
                    let toasterConfig= {
                        toasterText: serverData.text,
                        showToaster: true,
                    };
                    store.dispatch('updateToasterParams', toasterConfig);
                    setTimeout(()=>{
                        store.dispatch('updateToasterParams', {
                            showToaster: false
                        });
                    }, serverData.timeout);

                }
            };

            arrEventObj.forEach((action)=>{
                if(!systemActions[action.type]){
                    console.error(`Action type ${action.type} was not defined in User userActions`);
                }
                systemActions[action.type](action.data);
            });


        }
    },
    chat: {
        add: function(arrEventObj){
            arrEventObj.forEach((chatMessageToAdd)=>{
                store.dispatch("signalRAddMessage", chatMessageToAdd);
            });
        },
        update: function(arrEventObj){
            arrEventObj.forEach((conversationObj)=>{
                store.dispatch("checkUnreadMessageFromSignalR", conversationObj);
            });
        }
    },
    studyroom:{
        add:function(arrEventObj){
            arrEventObj.forEach((roomInfo)=>{
                store.dispatch("signalRAddRoomInformationMessage", roomInfo);
            });
        },
        update:function(arrEventObj){
            arrEventObj.forEach((roomStatusInformation)=>{
                store.dispatch("signalR_UpdateState", roomStatusInformation);
            });
        },
        action:function(arrEventObj){
            arrEventObj.forEach((sessionInformation)=>{
                store.dispatch("signalR_SetJwtToken", sessionInformation);
            });
        }
    }
};