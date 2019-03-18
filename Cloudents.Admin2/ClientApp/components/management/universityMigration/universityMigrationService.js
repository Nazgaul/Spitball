import { connectivityModule } from '../../../services/connectivity.module';


function universityItem(objInit) {
    this.newId = objInit.newId;
    this.newUniversity = objInit.newUniversity;
    this.oldId = objInit.oldId;
    this.oldUniversity = objInit.oldUniversity;
}
function createUniversityItem(objInit) {
    return new universityItem(objInit);
}

const path = 'AdminUniversity/';

const getUniversityList = function () {

    return connectivityModule.http.get(`${path}universities`).then((newUniversityList) => {
        let arrUniversityList = [];
        if (newUniversityList.length > 0) {
            newUniversityList.forEach((ci) => {
                arrUniversityList.push(createUniversityItem(ci));
            });
        }
        return Promise.resolve(arrUniversityList);
    }, (err) => {
        return Promise.reject(err);
    });
};

const migrateUniversities = function (universityToRemove, universityToKeep) {
    return connectivityModule.http.post(`${path}migrate`, {
        "universityToRemove": universityToRemove,
        "universityToKeep": universityToKeep
    })
        .then((resp) => {
            console.log(resp, 'post doc success');
            return Promise.resolve(resp);
        }, (error) => {
            console.log(error, 'error post doc');
            return Promise.reject(error);
        });
};

export {
    getUniversityList,
    migrateUniversities,
    createUniversityItem
};

