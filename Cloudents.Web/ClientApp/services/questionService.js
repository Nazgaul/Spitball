import axios from "axios";
import qs from "query-string";
import {dollarCalculate} from "../store/constants";

axios.defaults.paramsSerializer = params => qs.stringify(params, {indices: false});
axios.defaults.responseType = "json";
let transferResultQuestion = res => {
    return { ...res,filesNum:res.files.length,answersNum:res.answers.length,price:calculateDollar(res.price)}
};
let calculateDollar=(balance)=>dollarCalculate(balance).toFixed(2);
export default {
    deleteQuestion:({id,type})=> axios.delete(`/${type}/${id}`),
    getSubjects: () => axios.get("/Question/subject"),
    postQuestion: (subjectId, text, price, files) => axios.post("/Question", {subjectId, text, price, files}),
    getQuestion: (id) => axios.get("/Question/"+id,{transformResponse:transferResultQuestion}),
    answerQuestion: (questionId, text, files) => axios.post("/Answer", {questionId, text, files}),
    markAsCorrectAnswer: (answerId) => axios.put("/Question/correct", {answerId}),
    calculateDollar
}