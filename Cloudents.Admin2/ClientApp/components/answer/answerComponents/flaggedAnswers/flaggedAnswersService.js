import { connectivityModule } from '../../../../services/connectivity.module'


function FlaggedAnswerItem(objInit) {
    this.id = objInit.id;
    this.reason = objInit.reason;
    this.flaggedUserEmail = objInit.flaggedUserEmail;
    this.text = objInit.text || '';
    this.questionText = objInit.questionText || '';
    this.markerEmail = objInit.markerEmail || '';
}


function createFlaggedAnswerItem(objInit) {
    return new FlaggedAnswerItem(objInit);
}

const getAllAnswers = function () {
    let path = 'AdminAnswer/flagged';
    return connectivityModule.http.get(path).then((answers) => {
        let arrAnswers = [];
        if (answers.length > 0) {
            answers.forEach(function (answer) {
                arrAnswers.push(createFlaggedAnswerItem(answer));
            });
        }
        return Promise.resolve(arrAnswers);
    }, (err) => {
        return Promise.reject(err);
    });
};

const aproveAnswer = function (id) {
    let path = 'AdminAnswer/unFlag';
    let idObj = {
        id: id
    };
    return connectivityModule.http.post(path, idObj).then(() => {
        return Promise.resolve(true);
    }, (err) => {
        return Promise.reject(err);
    });
};


export {
    getAllAnswers,
    aproveAnswer,
}