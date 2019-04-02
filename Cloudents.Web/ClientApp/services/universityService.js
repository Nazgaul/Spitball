import { connectivityModule } from "./connectivity.module";
import { LanguageService } from "../services/language/languageService";

function University(objInit) {
    if(!objInit) {
        this.id = "";
        this.country = "";
        this.text = "";
    } else {
        this.id = objInit.id;
        this.country = objInit.country;
        this.text = objInit.name;
    }
}

function AddUniversityObj() {
    this.text = LanguageService.getValueByKey("uniSelect_didnt_find_university");
    this.helper = true;
}

// function addClassObj(){
//     this.text = LanguageService.getValueByKey("uniSelect_didnt_find_class");
//     this.helper = true;
// }

function Course(objInit) {
    this.text = objInit.name;
    this.isFollowing = objInit.isFollowing || false;
    this.isTeaching = objInit.isTeaching || false;
    this.students = objInit.students || 10;
    this.isPending = objInit.isPending || false;

}

function ServerCourse(name) {
    this.name = name;
}

const getUni = (val) => {
    return connectivityModule.http.get(`university?term=${val}`).then(({data}) => {
        let result = [];
        if(!!data.universities && data.universities.length > 0) {
            data.universities.forEach((uni) => {
                result.push(new University(uni));
            });
            result.push(new AddUniversityObj());
        }
        console.log(data);
        return result;
    }, (err) => {
        return Promise.reject(err);
    });
};

const assaignUniversity = (uniName) => {
    let university = {
        name: uniName
    };
    return connectivityModule.http.post("University/assign", university).then(() => {
        return true;
    }, (err) => {
        return Promise.reject(err);
    });
};

const getCourse = (val) => {
    let path = val ? `course/search?term=${val}` : `course/search`;
    return connectivityModule.http.get(`${path}`).then(({data}) => {
        let result = [];
        if(!!data.courses && data.courses.length > 0) {
            data.courses.forEach((course) => {
                result.push(new Course(course));
            });
            // result.push(new addClassObj());
        }
        return result;
    }, (err) => {
        return Promise.reject(err);
    });
};

const assaignCourse = (arrCourses) => {
    let courses = [];
    arrCourses.forEach(course => {
        if(typeof course === 'object') {
            courses.push(new ServerCourse(course.text));
        } else {
            courses.push(new ServerCourse(course));
        }
    });
    return connectivityModule.http.post("Course/set", courses);
};

const getProfileUniversity = () => {
    return connectivityModule.http.get("account/university").then(({data}) => {
        let result = new University(data);
        return result;
    });
};

const getProfileCourses = () => {
    return connectivityModule.http.get("account/courses").then(({data}) => {
        let result = [];
        if(!!data && data.length > 0) {
            data.forEach((course) => {
                result.push(new Course(course));
            });
        }
        return result;
    });
};
const deleteCourse = (name) => {
    return connectivityModule.http.delete(`course/${name}`).then((resp)=>{
        return resp
    })
};

export default {
    getUni,
    assaignUniversity,
    getCourse,
    assaignCourse,
    getProfileUniversity,
    getProfileCourses,
    deleteCourse
};