import { connectivityModule } from '../../../services/connectivity.module';


function courseItem(objInit) {
    this.name = objInit.name;
}
function createCourseItem(objInit) {
    return new courseItem(objInit);
}


const path = 'AdminCourse/';

const getSubjects = function () {
    return connectivityModule.http.get(`${path}subject`).then((subjects) => {
        let arrCourseList = [];
        if (subjects.length > 0) {
            subjects.forEach((ci) => {
                arrCourseList.push(ci);
            });
        }
        return Promise.resolve(arrCourseList);
    }, (err) => {
        return Promise.reject(err);
    });
};

const getCourseList = function (language, state) {
    return connectivityModule.http.get(`${path}newCourses?Language=${language}&State=${state}`).then((newCourseList) => {
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

const approve = function (course) {
    return connectivityModule.http.post(`${path}approve`, { "Course": course.name.name, "Subject": course.subject })
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