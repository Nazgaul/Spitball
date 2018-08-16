

export const signalREventHandler = {
    questions: {
        add: function(eventObj){
            $store.dispatch("addQuestionItem", eventObj);
        },
        delete: function(eventObj){
            $store.dispatch("removeQuestionItem", eventObj);
        }
    }
}