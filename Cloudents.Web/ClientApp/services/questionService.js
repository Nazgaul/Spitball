import { connectivityModule } from "./connectivity.module"
import searchService from './searchService'


function AnswerItem(objInit){
   this.create = objInit.create || objInit.Create;
   this.files = objInit.files || objInit.Files;
   this.id = objInit.id || objInit.Id;
   this.text = objInit.text || objInit.Text;
   this.user = objInit.user || objInit.User;
}

export default {
    deleteQuestion:({id,type}) => {
       return connectivityModule.http.delete(`/${type}/${id}`);
    },
    getSubjects: () => {
        let cacheControl = `?v=${global.version}&l=${global.lang}`;
        return connectivityModule.http.get(`/Question/subject${cacheControl}`)
    },
    postQuestion: (subjectId, text, price, files, color) => {
       return connectivityModule.http.post("/Question", {subjectId, text, price, files, color})
    },
    getQuestion: (id) => connectivityModule.http.get("/Question/"+id).then(({data}) => {
        let res = searchService.createQuestionItem(data);
        return { ...res, filesNum:res.files.length };
    }),
    answerQuestion: (questionId, text, files) => {
       return connectivityModule.http.post("/Answer", {questionId, text, files})
    },
    markAsCorrectAnswer: (answerId) => {
       return connectivityModule.http.put("/Question/correct", {answerId})
    },
    createAnswerItem: (objInit) => {
      return new AnswerItem(objInit);
    }
}