import axios from "axios";
import qs from "querystring";

axios.defaults.paramsSerializer=params => qs.stringify(params, { indices: false });
axios.defaults.responseType="json";
axios.defaults.baseURL="api/";

let transferResultNote=res=>{
    let result = res.result || [];
    return {source: res.facet,data:result.map(val => { return { ...val, template: "item" } })}
};
//todo think about error
let transferResultAsk=(res,header)=>{
    console.log(header);
    if(Number(header.status)!==200)return {data:[]};
    const video = res.video;
    const itemResult = res.result || [];
    const items = itemResult.map(val => { return { ...val, template: "item" } });
    const data = video ? [{...video,template:"video"},...items]:items;
    return {data}
};
let transferResultTutor=data=>{
    let body = data || [];
    return{ data: body.map(val => { return { ...val, template: "tutor" } }) };
};
let transferJob=body=>{
    let {result,facet:jobType}=body;
    return{ jobType, data: result.map(val => { return { ...val, template: "job" } }) };
};
let transferBook=body=>{
    body = body || [];
    let data =  body.map(val => { return { ...val, template: "book" } });
    return {data}
};
let transferFood=body=>{
    const data = body.data || [];
    return{ token: body.token, data: data.map(val => { return { ...val, template: "food" } }) };
};
let transferBookDetails=body=>{
    let prices = body.prices || [];
    return {details: body.details, data: prices.map(val => { return { ...val, template: "book-price" }})}
};

const searchFunctions = {
    getDocument:(params)=>axios.get("search/documents",{params,transformResponse:transferResultNote}),
    getQna: (params)=>axios.get("ask",{params,transformResponse:transferResultAsk}),
    getFlashcard:(params)=>axios.get("search/flashcards",{params,transformResponse:transferResultNote}),
    getTutor: (params)=>axios.get("tutor",{params,transformResponse:transferResultTutor}),
    getJob: (params)=>axios.get("job",{params,transformResponse:transferJob}),
    getBook: (params)=>axios.get("book/search",{params,transformResponse:transferBook}),
    getBookDetails: ({type,isbn13})=>axios.get(`book/${type}`,{params:{isbn13},transformResponse:transferBookDetails}),
    getFood: (params)=>axios.get("places",{params,transformResponse:transferFood})
};

const courseFunctions = {
    getCourse: (params)=>axios.get("course",{params}),
    createCourse: (data)=>axios.post('course',qs.stringify(data))
};
export const interpetPromise=(sentence) =>axios.get("AI",{params:{sentence}});
const getBookDetails=({type,isbn13})=>axios.get(`book/${type}`,{params:{isbn13}});
export const getUniversity = (params)=>axios.get("university",{params});
export const search = {getBookDetails,...searchFunctions};
export const course = {...courseFunctions};
export const flashcard = {};