import { connectivityModule } from '../../../services/connectivity.module';


function CourseItem(objInit) {
    this.name = objInit.name;
}

function createCourseItem(objInit) {
    return new CourseItem(objInit);
}

function SubjectItem (objInit){
    this.text  = objInit.name;
    this.value = objInit.id;
}

/**
 * 
 * @param {array} init 
 * @returns {SubjectItem[]}
 */
function mapSubjects(init) {
    return init.map(x=> new SubjectItem(x));
}


const path = 'AdminCourse/';

const getSubjects = function () {

   /**
    * @param {array} subject
    */
    return connectivityModule.http.get(`AdminSubject`).then(mapSubjects);
       
        // let arrCourseList = ["N/A"];
        //     subjects.forEach((ci) => {
        //         arrCourseList.push(ci);
        //     });
        // return arrCourseList;
    //});
};

const getCourseList = function ( state, filter) {
    let query = `?State=${state}`;

    if(filter){
        query += `&Search=${filter}`;
    }
        return connectivityModule.http.get(`AdminCourse${query}`).then((newCourseList) => {
            let arrCourseList = [];
            if (newCourseList.length > 0) {
                newCourseList.forEach((ci) => {
                    arrCourseList.push(createCourseItem(ci));
                });
            }
            return arrCourseList;
        });

};
const createCourse = function (obj) {
    return connectivityModule.http.post(`${path}`, { "Name": obj.name, "Subject": obj.subject });

}

const migrateCourses = function (newCourse, oldCourse) {
    return connectivityModule.http.post(`${path}migrate`, { "CourseToRemove": newCourse, "CourseToKeep": oldCourse });
        
};

const approve = function ({course, schoolType}) {
    return connectivityModule.http.post(`${path}approve`, 
    { "Course": course.name.name, "Subject": course.subject !== "N/A" ? course.subject : null });
        
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
    return connectivityModule.http.delete(`${path}${encodeURIComponent(course.name)}`)
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
    createCourseItem,
    createCourse
};