import { connectivityModule } from "./connectivity.module"
import searchService from './searchService'

export default {
   delete:({id,type}) => connectivityModule.http.delete(`/${type}/${id}`),
   post:({course, text}) => connectivityModule.http.post("/Question", {course, text}),
   get:(id) => connectivityModule.http.get("/Question/"+id).then(({data}) => {
      let res = searchService.createQuestionItem(data);
      return { ...res};
   }),
   answer:(questionId, text, files) => connectivityModule.http.post("/Answer", {questionId, text, files}),
}