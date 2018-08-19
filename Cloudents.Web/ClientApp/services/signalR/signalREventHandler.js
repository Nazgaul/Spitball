import store from '../../store/index'

const signalREventHandler = {
    questions: {
        add: function(eventObj){
            store.dispatch("addQuestionItemaaaaaaa", eventObj);
        },
        delete: function(eventObj){
            store.dispatch("removeQuestionItem", eventObj);
        }
    }
}

export const signlaREvents = {
    "NT_QUESTION_ADDED": signalREventHandler.questions.add,
}