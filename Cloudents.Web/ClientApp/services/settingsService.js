import { transformLocation, course } from './resources';
import { connectivityModule } from "./connectivity.module"
// import { getUniversity, course } from './resources';


const getUni = (params) => {
    return connectivityModule.http.get("university", { params: transformLocation(params) })
};

const assaignUniversity = (universityId) => {
    let data = {
        "universityId": universityId
    }
    return connectivityModule.http.post("University/assign", data)
}

const createUniversity = (universityName)=>{
    let data = {
        "name": universityName
    }
    return connectivityModule.http.post("University", data)
}

const getCourse = (params) => {
    return connectivityModule.http.get("course/search", { params })
};

const createCourse = (data) => {
    return connectivityModule.http.post("course/create", data)
};


export default {
    getUniversity({term,location}) {
        return getUni({term,location});
    },
    getCourse(params) {
        return getCourse(params);
    },
    createCourse(model) {
        return createCourse(model);
    },
    assaignUni(universityId){
       return assaignUniversity(universityId)
    },
    createUni(universityName){
        return createUniversity(universityName)
    }
}