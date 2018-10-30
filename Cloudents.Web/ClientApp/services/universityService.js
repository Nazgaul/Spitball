import { connectivityModule } from "./connectivity.module"

function University(objInit){
    this.id = objInit.id;
    this.country = objInit.country;
    this.text = objInit.name;
}

function Course(objInit){
    this.text = objInit.name;
}
function ServerCourse(name){
    this.name = name
}

const getUni = (val) => {
    return connectivityModule.http.get(`university?term=${val}`).then(({data})=>{
        let result = [];
        if(!!data.universities && data.universities.length > 0){
            data.universities.forEach((uni)=>{
                result.push(new University(uni));
            });
        }
        console.log(data);
        return result;
    },(err)=>{
        return err;
    })
};

const assaignUniversity = (uniName) => {
    let university = {
        name: uniName
    }
    return connectivityModule.http.post("University/assign", university).then(()=>{
        return true;
    }, (err)=>{
        return err;
    })
}

const getCourse = (val) => {
    return connectivityModule.http.get(`course/search?term=${val}`).then(({data})=>{
        let result = [];
        if(!!data.courses && data.courses.length > 0){
            data.courses.forEach((course)=>{
                result.push(new Course(course));
            });
        }
        return result;
    }, (err)=>{
        return err;
    })
};

const assaignCourse = (arrCourses) => {
    let courses = [];
    arrCourses.forEach(course=>{
        if(typeof course === 'object'){
            courses.push(new ServerCourse(course.text))
        }else{
            courses.push(new ServerCourse(course))
        }
    })
    return connectivityModule.http.post("Course/assign", courses)
}

const getProfileUniversity = () => {
    return connectivityModule.http.get("Profile/university").then(({data})=> {
        let result = new University(data[0]); 
        return result;
    })
}

const getProfileCourses = () => {
    return connectivityModule.http.get("Profile/courses").then(({data})=> {
        let result = [];
        if(!!data && data.length > 0){
            data.forEach((course)=>{
                result.push(new Course(course));
            });
        }
        return result;
    })
}

export default {
    getUni,
    assaignUniversity,
    getCourse,
    assaignCourse,
    getProfileUniversity,
    getProfileCourses
}