import { connectivityModule } from "./connectivity.module"
import searchService from './searchService'

export default {
    deleteQuestion:({id,type}) => {
       return connectivityModule.http.delete(`/${type}/${id}`);
    },
    postQuestion: ({course, text}) => {
       return connectivityModule.http.post("/Question", {course, text});
    },
    getQuestion: (id) => connectivityModule.http.get("/Question/"+id).then(({data}) => {
        let res = searchService.createQuestionItem(data);
        return { ...res};
    }),
    answerQuestion: (questionId, text, files) => {
       return connectivityModule.http.post("/Answer", {questionId, text, files});
    },
}