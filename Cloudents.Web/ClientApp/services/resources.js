import axios from "axios";
import qs from "query-string";

axios.defaults.paramsSerializer = params => qs.stringify(params, { indices: false });
axios.defaults.responseType = "json";
axios.defaults.baseURL = "api";
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
    return { source: res.facet,facet:res.facet, data: result.map(val => { return { ...val, template: "item" } }) }
};
//todo think about error
let transferResultAsk = res => {
    const video = res.video;
    const itemResult = res.result.result || [];
    const items = itemResult.map(val => { return { ...val, template: "item" } });
    const data = video ? [{ ...video, template: "video" }, ...items] : items;
    return { data, source:res.result.facet,facet:res.result.facet}
};
let transferResultTutor = data => {
    let body = data || [];
    return { data: body.map(val => { return { ...val, template: "tutor" } }) };
};
let transferJob = body => {
    let { result, facet: jobType,facet } = body;
    return { jobType,facet, data: result.map(val => { return { ...val, template: "job" } }) };
};
let transferBook = body => {
    body = body || [];
    let data = body.map(val => { return { ...val, template: "book" } });
    return { data }
};
let transferFood = body => {
    const data = body.data || [];
    return { token: body.token, data: data.map(val => { return { ...val, template: "food" } }) };
};
let transferBookDetails = body => {
    let prices = body.prices || [];
    return { details: body.details, data: prices.map(val => { return { ...val, template: "book-price" } }) }
};

const searchFunctions = {
    getDocument: (params) => axios.get("search/documents", {params, transformResponse: transferResultNote }),
    getQna: (params) => axios.get("ask", { params, transformResponse: transferResultAsk }),
    getFlashcard: (params) => axios.get("search/flashcards", { params, transformResponse: transferResultNote }),
    getTutor: (params) => axios.get("tutor", { params:transformLocation(params), transformResponse: transferResultTutor}),
    getJob: (params) => axios.get("job", { params:transformLocation(params),transformResponse: transferJob}),
    getBook: (params) => axios.get("book/search", { params, transformResponse: transferBook }),
    getBookDetails: ({ type, isbn13 }) => axios.get(`book/${type}`, { params: { isbn13 }, transformResponse: transferBookDetails }),
    getFood: (params) => axios.get("places", {params:transformLocation(params), transformResponse: transferFood }),
};

const courseFunctions = {
    getCourse: (params) => axios.get("course", { params }),
    createCourse: (data) => axios.post('course', qs.stringify(data))
};
export const interpetPromise = (sentence) => axios.get("AI", { params: { sentence } });
const getBookDetails = ({ type, isbn13 }) => axios.get(`book/${type}`, { params: { isbn13 } });
const getPlacesDetails = ({ id }) => {
    debugger;
    return axios.get("places", { params: { id } });
}
export const getUniversity = (params) => axios.get("university", { params:transformLocation(params) });

export const search = { getBookDetails, ...searchFunctions, getPlacesDetails,autoComplete:(data)=>axios.get("suggest",{params:{sentence:data.term, vertical: data.vertical}}) };
export const course = { ...courseFunctions };
export const help = {
    getFaq: () => axios.get("help"),
    getUniData: (id) => axios.get("blog", { params: { id: id } })
};

export const spitballPreview = {
    getFlashcard: ({ id }) => axios.get("flashcard", { params: { id: Number(id) } }),
    getDocument: ({ id }) => axios.get("document", { params: { id: Number(id) } })
};