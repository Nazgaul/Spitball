import { connectivityModule } from "./connectivity.module"

function University(objInit){
    this.id = objInit.id;
    this.country = objInit.country;
    this.text = objInit.name;
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
    return connectivityModule.http.get("course/search", val)
};

const assaignCourse = (courseName) => {
    let course = {
        name: courseName
    }
    return connectivityModule.http.post("Course/assign", course)
}

export default {
    getUni,
    assaignUniversity,
    getCourse,
    assaignCourse
}