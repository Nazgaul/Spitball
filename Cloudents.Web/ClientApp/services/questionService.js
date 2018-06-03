import axios from "axios";
import qs from "query-string";

axios.defaults.paramsSerializer = params => qs.stringify(params, {indices: false});
axios.defaults.responseType = "json";
export default {
    getSubjects: () => axios.get("/Question/subject"),
    postQuestion: (subjectId, text, price, privateKey, files) => axios.post("/Question", {subjectId, text, price, privateKey, files}),
    getQuestion: (id) => axios.get("/Question/"+id),
    answerQuestion: (questionId, text, files) => axios.post("/Answer", {questionId, text, files}),
    markAsCorrectAnswer: (answerId) => axios.put("/Question/correct", {answerId}),
    upVote:(id) => axios.post("/Answer/upVote", {id}),

}