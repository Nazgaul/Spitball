import { connectivityModule } from '../../../../services/connectivity.module'


function PendingAnswerItem(objInit) {
    this.id = objInit.id;
    this.text = objInit.text;
    this.imagesCount = objInit.imagesCount;
    this.user = {
        id: objInit.userId,
        email: objInit.email
    }
}


function createPendingAnswerItem(objInit) {
    return new PendingAnswerItem(objInit);
}

const getAllAnswers = function () {
    let path = 'AdminAnswer/Pending'
    return connectivityModule.http.get(path).then((answers) => {
        let arrAnswers = [];
        if (answers.length > 0) {
            answers.forEach(function (answer) {
                arrAnswers.push(createPendingAnswerItem(answer));
            })
        }
        return Promise.resolve(arrAnswers)
    }, (err) => {
        return Promise.reject(err)
    })
}

const aproveAnswer = function (id) {
    let path = 'AdminAnswer/approve'
    let idObj = {
        id: id
    }
    return connectivityModule.http.post(path, idObj).then(() => {
        return Promise.resolve(true)
    }, (err) => {
        return Promise.reject(err)
    })
}


export {
    getAllAnswers,
    aproveAnswer,
}