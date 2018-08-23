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
        addviewr: function(arrEventObj){
            arrEventObj.forEach((question)=>{
                store.dispatch("addQuestionViewr", question);
            })
        },
        removeviewr: function(arrEventObj){
            arrEventObj.forEach((question)=>{
                store.dispatch("removeQuestionViewr", question);
            })
        },
    }
}