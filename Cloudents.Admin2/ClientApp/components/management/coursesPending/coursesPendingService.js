import { connectivityModule } from '../../../services/connectivity.module';


function CourseItem(objInit) {
    this.name = objInit.name;
}
function createCourseItem(objInit) {
    return new CourseItem(objInit);
}


const path = 'AdminCourse/';

const getSubjects = function () {
    return connectivityModule.http.get(`${path}subject`).then((subjects) => {
        let arrCourseList = ["N/A"];
            subjects.forEach((ci) => {
                arrCourseList.push(ci);
            });
        return arrCourseList;
    });
};

const getCourseList = function (language, state, filter) {
    language = language === '' ? null : language;
    let query = `?Language=${language}&State=${state}`;

    if(filter){
        query += `&Filter=${filter}`;
    }
        return connectivityModule.http.get(`${path}newCourses${query}`).then((newCourseList) => {
            let arrCourseList = [];
            if (newCourseList.length > 0) {
                newCourseList.forEach((ci) => {
                    arrCourseList.push(createCourseItem(ci));
                });
            }
            return Promise.resolve(arrCourseList);
        }, (err) => {
            return Promise.reject(err);
        });

};

const migrateCourses = function (newCourse, oldCourse) {
    return connectivityModule.http.post(`${path}migrate`, { "CourseToRemove": newCourse, "CourseToKeep": oldCourse })
        .then((resp) => {
            console.log(resp, 'post doc success');
            return Promise.resolve(resp);
        }, (error) => {
            console.log(error, 'error post doc');
            return Promise.reject(error);
        });
};

const approve = function ({course, schoolType}) {
    return connectivityModule.http.post(`${path}approve`, { "Course": course.name.name, "Subject": course.subject !== "N/A" ? course.subject : null, schoolType })
        .then((resp) => {
            console.log(resp, 'post doc success');
            return Promise.resolve(resp);
        }, (error) => {
            console.log(error, 'error post doc');
            return Promise.reject(error);
        });
};

const rename = function (course, newName) {
    return connectivityModule.http.post(`${path}rename`, { "OldName": course, "NewName": newName })
        .then((resp) => {
            console.log(resp, 'post doc success');
            return Promise.resolve(resp);
        }, (error) => {
            console.log(error, 'error post doc');
            return Promise.reject(error);
        });
};

const deleteCourse = function (course) {
    return connectivityModule.http.delete(`${path}${course.name}`)
        .then((resp) => {
            console.log(resp, 'post doc success');
            return Promise.resolve(resp);
        }, (error) => {
            console.log(error, 'error post doc');
            return Promise.reject(error);
        });
};

export {
    getCourseList,
    getSubjects,
    //getSuggestions,
    approve,
    rename,
    deleteCourse,
    migrateCourses,
    createCourseItem
};