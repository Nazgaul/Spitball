import axios from "axios";
import qs from "query-string";

axios.defaults.paramsSerializer = params => qs.stringify(params, {indices: false});
axios.defaults.responseType = "json";
export default {
    getSubjects: () => axios.get("/Question/subject"),
    postQuestion: (subjectId, text, price) => axios.post("/Question", {subjectId, text, price}),
    getQuestion: (id) => axios.get("/Question/"+id),
    answerQuestion: (questionId, text, files) => axios.post("/Answer", {questionId, text, files})
}