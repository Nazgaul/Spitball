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

const createUni = (uni) => {
    return connectivityModule.http.post("university/create", {name: uni}).then(() => {
        return uni;
    });
};

export default {
    getUni,
    assaignUniversity,
    createUni,
};