import { transformLocation } from './resources';
import { connectivityModule } from "./connectivity.module"
// import { getUniversity, course } from './resources';


const getUni = (params) => {
    return connectivityModule.http.get("university", { params: transformLocation(params) });
};

const assaignUniversity = (universityId) => {
    let data = {
        "universityId": universityId
    };
    return connectivityModule.http.post("University/assign", data);
};

const createUniversity = (universityName)=>{
    let data = {
        "name": universityName
    };
    return connectivityModule.http.post("University", data);
};

const getCourse = (params) => {
    return connectivityModule.http.get("course/search", { params });
};

const createCourse = (data) => {
    return connectivityModule.http.post("course/create", data);
};

const assaignCourse = (courseId) => {
    let assignCourseData = {
        courseId
    };
    return connectivityModule.http.post("Course/assign", assignCourseData);
};


export default {
    getCourse(params) {
        return getCourse(params);
    },
    assaignCou(courseId){
        return assaignCourse(courseId);
    },
    createCourse(model) {
        return createCourse(model);
    },
    getUniversity({term,location}) {
        return getUni({term,location});
    },
    assaignUni(universityId){
       return assaignUniversity(universityId);
    },
    createUni(universityName){
        return createUniversity(universityName);
    }
}