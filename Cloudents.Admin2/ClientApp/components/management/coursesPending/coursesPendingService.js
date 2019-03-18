import { connectivityModule } from '../../../services/connectivity.module';


function courseItem(objInit) {
    this.name = objInit.name;
}
function createCourseItem(objInit) {
    return new courseItem(objInit);
}


const path = 'AdminCourse/';

const getSuggestions = function (item) {
    return connectivityModule.http.get(`${path}search?course=${item}`).then((suggestCourses) => {
        let arrCourseList = [];
        if (suggestCourses.courses.length > 0) {
            suggestCourses.courses.forEach((ci) => {
                arrCourseList.push(createCourseItem(ci).name);
            });
        }
        return Promise.resolve(arrCourseList);
    }, (err) => {
        return Promise.reject(err);
    });
};

const getCourseList = function () {
    return connectivityModule.http.get(`${path}newCourses`).then((newCourseList) => {
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
    return connectivityModule.http.post(`${path}approve`, { "Course": course.name })
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
    getSuggestions,
    approve,
    deleteCourse,
    migrateCourses,
    createCourseItem
};