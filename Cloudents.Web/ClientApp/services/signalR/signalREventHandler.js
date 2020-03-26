import store from '../../store/index'

export const signlaREvents = {
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
                paymentreceived: function(){
                    store.dispatch("signalR_ReleasePaymeStatus");
                },
                enterstudyroom: function(data){
                    store.dispatch("signalR_TutorEnterStudyRoom", data);
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
    studyroom:{
        add:function(arrEventObj){
            arrEventObj.forEach((roomInfo)=>{
                store.dispatch("signalRAddRoomInformationMessage", roomInfo);
            });
        },
        update:function(arrEventObj){
            arrEventObj.forEach((roomStatusInformation)=>{
                // TODO: remove it
                if(!store.getters.getJwtToken){
                    store.dispatch('updateJwtToken',roomStatusInformation.jwtToken)
                }
                // TODO: remove it

                // store.dispatch("signalR_UpdateState", roomStatusInformation);
            });
        },
        action:function(arrEventObj){
            arrEventObj.forEach((sessionInformation)=>{
                // TODO: remove it
                if(!store.getters.getJwtToken){
                    store.dispatch('updateJwtToken',sessionInformation.data.jwtToken)
                }
                // TODO: remove it

                // store.dispatch("signalR_SetJwtToken", sessionInformation);
            });
        }
    }
};