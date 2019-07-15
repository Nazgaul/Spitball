import { connectivityModule } from '../../../../services/connectivity.module';

function createSessionItem(objInit) {
    return new CreateSessionItem(objInit);
}

function CreateSessionItem(objInit) {
    this.tutorName = objInit.tutorName;
    this.userName = objInit.userName;
    this.created = new Date(objInit.created);
    this.duration = objInit.duration;
    this.tutorId = objInit.tutorId;
    this.userId = objInit.userId;
}

const path = 'AdminStudyRoom/';

const getSessions = function () {
    return connectivityModule.http.get(`${path}`).then((newSessionsList) => {
        let arrSessionsList = [];
        if (newSessionsList.length > 0) {
            newSessionsList.forEach((Session) => {
                arrSessionsList.push(createSessionItem(Session));
            });
        }
        return Promise.resolve(arrSessionsList);
    }, (err) => {
        return Promise.reject(err);
    });
};

export {
    getSessions
};