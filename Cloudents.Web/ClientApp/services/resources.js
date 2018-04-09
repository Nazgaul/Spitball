import axios from "axios";
import qs from "query-string";

axios.defaults.paramsSerializer = params => qs.stringify(params, { indices: false });
axios.defaults.responseType = "json";
axios.defaults.baseURL = window.baseURL;
let currentVertical='item';
const itemVerticals=['ask','flashcard','note'];
const getTemplate=(val)=>itemVerticals.includes(val)?'item':val;
let transformLocation=(params)=>{
    let location=params.location;
    delete params.location;
    if(location){
        params['location.latitude']=location.latitude;
        params['location.longitude']=location.longitude;
    }
    return params;
};
let transferResultNote = res => {
    let result = res?res.result:[];
    if(!res) return {data:[]};
    return { source: res.facet,facet:res.facet, data: result.map(val => { return { ...val, template: 'item' } }),nextPage:res.nextPageLink }
};
let transferNextPage=(res)=>{
    let {data,nextPage}=transferMap[currentVertical](res);
    return { data,nextPage}
};
//todo think about error
let transferResultAsk = res => {
    const video = res.video;
    const itemResult = res.result.result || [];
    const items = itemResult.map(val => { return { ...val, template: "item" } });
    const data = video ? [{ ...video, template: "video" }, ...items] : items;
    return { data, source:res.result.facet,facet:res.result.facet,nextPage:res.nextPageLink}
};
let transferResultTutor = data => {
    let body = data || {};
    return { data: body.result.map(val => { return { ...val, template: "tutor" } }) ,nextPage:body.nextPageLink};
};
let transferJob = body => {
    let { result,nextPageLink:nextPage } = body;
    let {result:items, facet: jobType}=result;
    return { jobType, data: items.map(val => { return { ...val, template: "job" } }),nextPage };
};
let transferBook = body => {
    body = body || {};
    let data = body.result.map(val => { return { ...val, template: "book" } });
    return { data,nextPage:body.nextPageLink }
};
let transferFood = body => {
    const data = body.data || [];
    return { token: body.token, data: data.map(val => { return { ...val, template: "food" } }),nextPage:body.nextPageLink };
};
let transferBookDetails = body => {
    let prices = body.prices || [];
    return { details: body.details, data: prices.map(val => { return { ...val, template: "book-price" } }) }
};
const transferMap={
    ask:(res)=>transferResultAsk(res),
    flashcard:(res)=>transferResultNote(res),
    note:(res)=>transferResultNote(res),
    job:(res)=>transferJob(res),
    food:(res)=>transferFood(res),
    tutor:(res)=>transferResultTutor(res),
    book:(res)=>transferBook(res)
}
const searchFunctions = {
    getDocument: (params) => axios.get("search/documents", {params, transformResponse: transferResultNote }),
    getQna: (params) => axios.get("ask", { params, transformResponse: transferResultAsk }),
    getFlashcard: (params) => axios.get("search/flashcards", { params, transformResponse: transferResultNote }),
    getTutor: (params) => axios.get("tutor", { params:transformLocation(params), transformResponse: transferResultTutor}),
    getJob: (params) => axios.get("job", { params:transformLocation(params),transformResponse: transferJob}),
    getBook: (params) => axios.get("book/search", { params, transformResponse: transferBook }),
    getBookDetails: ({ type, isbn13 }) => axios.get(`book/${type}`, { params: { isbn13 }, transformResponse: transferBookDetails }),
    getFood: (params) => axios.get("places", {params:transformLocation(params), transformResponse: transferFood }),
    getNextPage:({url,vertical})=>{
        currentVertical=vertical;
        return axios.get(url,{baseURL:"",transformResponse:transferNextPage})}
}

const courseFunctions = {
    getCourse: (params) => axios.get("course/search", { params }),
    createCourse: (data) => axios.post("course/create", qs.stringify(data))
};
export const interpetPromise = (sentence) => axios.get("AI", { params: { sentence } });
const getBookDetails = ({ type, isbn13 }) => axios.get(`book/${type}`, { params: { isbn13 } });
const getPlacesDetails = ({ id }) => {
    return axios.get("places", { params: { id } });
}
export const getUniversity = (params) => axios.get("university", { params:transformLocation(params) });
export const search = { getBookDetails, ...searchFunctions, getPlacesDetails,autoComplete:(data)=>axios.get("suggest",{params:{sentence:data.term, vertical: data.vertical}}) };
export const course = { ...courseFunctions };
export const help = {
    getFaq: () => axios.get("help",{baseURL:"api/"}),
    getUniData: (id) => axios.get("blog", {baseURL:"api/", params: { id: id } })
};

export const spitballPreview = {
    getFlashcard: ({ id }) => axios.get("flashcard", {params: { id: Number(id) } }),
    getDocument: ({ id }) => axios.get("document", {baseURL:"api/", params: { id: Number(id) } })};