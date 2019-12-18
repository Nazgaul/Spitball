import { connectivityModule } from "./connectivity.module";

function University(objInit) {
    if(!objInit) {
        this.id = "";
        this.country = "";
        this.text = "";
        this.students = "";
        this.image = "";
    } else {
        this.id = objInit.id;
        this.country = objInit.country;
        this.text = objInit.name;
        this.students = objInit.usersCount || 0;
        this.image = objInit.image || '';
    }
}
function Course(objInit) {
    this.text = objInit.name;
    this.isFollowing = objInit.isFollowing || false;
    this.isTeaching = objInit.isTeaching || false;
    this.students = objInit.students || 10;
    this.isPending = objInit.isPending || false;
    this.isLoading =  false;


}

function ServerCourse(name) {
    this.name = name;
}

const getUni = (val) => {
    return connectivityModule.http.get(`university?term=${val.term}&page=${val.page}`).then(({data}) => {
        let result = [];
        if(!!data.universities && data.universities.length > 0) {
            data.universities.forEach((uni) => {
                result.push(new University(uni));
            });
        }
        return result;
    }, (err) => {
        return Promise.reject(err);
    });
};

const assaignUniversity = (uniName) => {
    let university = {
        id: uniName
    };
    return connectivityModule.http.post("University/set", university).then(() => {
        return true;
    }, (err) => {
        return Promise.reject(err);
    });
};

const getCourse = (val) => {
    let path = val ? `course/search?term=${val.term}&page=${val.page}` : `course/search`;
    return connectivityModule.http.get(`${path}`).then(({data}) => {
        let result = [];
        if(!!data.courses && data.courses.length > 0) {
            data.courses.forEach((course) => {
                result.push(new Course(course));
            });
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
    return connectivityModule.http.delete(`course?name=${encodeURIComponent(name)}`).then((resp)=>{
        return resp;
    });
};
const createCourse = (course) => {
    return connectivityModule.http.post("course/create", course).then((resp) => {
        // return resp
        let createdCourse ={
            name: resp.data.name,
            isFollowing :true,
            isTeaching :  false,
            isPending: true,
        };
        return new Course(createdCourse);
    });
};
const createUni = (uni) => {
    return connectivityModule.http.post("university/create", {name: uni}).then(() => {
        // return resp
        // let createdUniversity ={
        //     name: resp.data.name,
        // };
        return uni;
    });
};

const teachCourse = (course) => {
    return connectivityModule.http.post("course/teach", {name: course}).then((resp) => {
        return resp;
    });
};

export default {
    getUni,
    assaignUniversity,
    getCourse,
    assaignCourse,
    getProfileUniversity,
    getProfileCourses,
    deleteCourse,
    createCourse,
    createUni,
    teachCourse,
};