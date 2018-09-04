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
            })
        },
        addviewr: function(question){
            store.dispatch("addQuestionViewer", question);
        },
        removeviewer: function(question){
            store.dispatch("removeQuestionViewer", question);
        },
    }
}