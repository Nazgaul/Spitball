import { connectivityModule } from "./connectivity.module"


export default {
    deleteQuestion:({id,type}) => {
       return connectivityModule.http.delete(`/${type}/${id}`);
    },
    getSubjects: () => {
        return connectivityModule.http.get("/Question/subject")
    },
    postQuestion: (subjectId, text, price, files) => {
       return connectivityModule.http.post("/Question", {subjectId, text, price, files})
    },
    getQuestion: (id) => connectivityModule.http.get("/Question/"+id).then(({data}) => {
        let res = data;
        return { ...res,filesNum:res.files.length, answersNum:res.answers.length }
    }),
    answerQuestion: (questionId, text, files) => {
       return connectivityModule.http.post("/Answer", {questionId, text, files})
    },
    markAsCorrectAnswer: (answerId) => {
       return connectivityModule.http.put("/Question/correct", {answerId})
    },
}