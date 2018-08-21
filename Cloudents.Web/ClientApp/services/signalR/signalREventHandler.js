import store from '../../store/index'

export const signlaREvents = {
    question: {
        add: function(arrEventObj){
            arrEventObj.forEach((questionToAdd)=>{
                store.dispatch("addQuestionItemAction", questionToAdd);
            })
            
        },
        delete: function(eventObj){
            store.dispatch("removeQuestionItem", eventObj);
        }
    }
}