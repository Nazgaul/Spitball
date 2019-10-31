import { connectivityModule } from '../../../services/connectivity.module';


function UniversityItem(objInit) {
    this.id = objInit.id;
    this.name = objInit.name;
    this.canBeDeleted = objInit.canBeDeleted;
}
function createUniversityItem(objInit) {
    return new UniversityItem(objInit);
}


const path = 'AdminUniversity/';

/*const getSuggestions = function (item) {
    return connectivityModule.http.get(`${path}search?university=${item}`).then((suggestUniversities) => {
        let arrCourseList = [];
        if (suggestUniversities.universities.length > 0) {
            suggestUniversities.universities.forEach((ci) => {
                arrCourseList.push(createUniversityItem(ci));
            });
        }
        return Promise.resolve(arrCourseList);
    }, (err) => {
        return Promise.reject(err);
    });
};*/

const getUniversitiesList = function (country, state) {
    return connectivityModule.http.get(`${path}newUniversities?Country=${country}&State=${state}`).then((newUniversitiesList) => {
        let arrCourseList = [];
        if (newUniversitiesList.length > 0) {
            newUniversitiesList.forEach((ci) => {
                arrCourseList.push(createUniversityItem(ci));
            });
        }
        return Promise.resolve(arrCourseList);
    }, (err) => {
        return Promise.reject(err);
    });
};

const migrateUniversities = function (uniToRemove, uniToKeep) {
    return connectivityModule.http.post(`${path}migrate`, { "UniversityToRemove": uniToRemove, "UniversityToKeep": uniToKeep })
        .then((resp) => {
            console.log(resp, 'post doc success');
            return Promise.resolve(resp);
        }, (error) => {
            console.log(error, 'error post doc');
            return Promise.reject(error);
        });
};

const approve = function (university) {
    return connectivityModule.http.post(`${path}approve`, { "Id": university.id })
        .then((resp) => {
            console.log(resp, 'post doc success');
            return Promise.resolve(resp);
        }, (error) => {
            console.log(error, 'error post doc');
            return Promise.reject(error);
        });
};

const rename = function (university, newName) {
    return connectivityModule.http.post(`${path}rename`, { "UniversityId": university, "NewName": newName })
        .then((resp) => {
            console.log(resp, 'post doc success');
            return Promise.resolve(resp);
        }, (error) => {
            console.log(error, 'error post doc');
            return Promise.reject(error);
        });
};

const deleteUniversity = function (university) {
    return connectivityModule.http.delete(`${path}${university.id}`)
        .then((resp) => {
            console.log(resp, 'post doc success');
            return Promise.resolve(resp);
        }, (error) => {
            console.log(error, 'error post doc');
            return Promise.reject(error);
        });
};

export {
    getUniversitiesList,
    //getSuggestions,
    approve,
    rename,
    deleteUniversity,
    migrateUniversities,
    createCourseItem
};