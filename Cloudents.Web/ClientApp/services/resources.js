import axios from "axios";
import qs from "query-string";
import { connectivityModule } from "./connectivity.module"

axios.defaults.paramsSerializer = params => qs.stringify(params, { indices: false });
axios.defaults.responseType = "json";
axios.defaults.baseURL = '/api';
let currentVertical = 'item';
const itemVerticals = ['flashcard', 'note'];
const getTemplate = (val) => itemVerticals.includes(val) ? 'item' : val;
let transformLocation = (params) => {
    let location = params.location;
    delete params.location;
    if (location) {
        params['location.point.latitude'] = location.latitude;
        params['location.point.longitude'] = location.longitude;
    }
    return params;
};
/*
const getBookDetails = ({ type, isbn13 }) => axios.get(`book/${type}`, { params: { isbn13 } });

*/
const courseFunctions = {
    getCourse: (params) => axios.get("course/search", { params }),
    createCourse: (data) => axios.post("course/create", data)
};
const getPlacesDetails = ({ id }) => {
    return axios.get("places/id", { params: { id } });
}
export const getUniversity = (params) => axios.get("university", { params: transformLocation(params) });

export const course = { ...courseFunctions };

export const spitballPreview = {
    getFlashcard: ({ id }) => axios.get("flashcard", { params: { id: Number(id) } }),
    getDocument: ({ id }) => axios.get("document", { baseURL: "api/", params: { id: Number(id) } })
};
