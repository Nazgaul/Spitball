import { connectivityModule } from '../../../services/connectivity.module';


function CourseItem(objInit) {
    this.newCourse = objInit.newCourse;
    this.oldCourse = objInit.oldCourse;
}
function createCourseItem(objInit) {
    return new CourseItem(objInit);
}

const path = 'AdminCourse/';

const getCourseList = function () {
    
    return connectivityModule.http.get(`${path}courses`).then((newCourseList) => {
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

export {
    getCourseList,
    migrateCourses,
    createCourseItem
};

