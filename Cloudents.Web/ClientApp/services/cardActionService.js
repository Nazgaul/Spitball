import { connectivityModule } from "./connectivity.module"

const reportQuestionItem = (data) => {
    console.log('got to send report stuff service')
    return connectivityModule.http.post("Question/flag", data)
};

const reportDocumentItem = (data) => {
    console.log('got to send report stuff service')
    return connectivityModule.http.post("Document/flag", data)
};


export default {
    reportQuestion(data) {
        return reportQuestionItem(data)
    },
    reportDocument(data) {
        return reportDocumentItem(data)
    },
}