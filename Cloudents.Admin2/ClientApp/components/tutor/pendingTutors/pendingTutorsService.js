import { connectivityModule } from '../../../services/connectivity.module'


function PendingTutorItem(objInit) {
    this.id = objInit.id;
    this.firstName = objInit.firstName;
    this.lastName = objInit.lastName;
    this.bio = objInit.bio;
    this.price = objInit.price;
    this.email = objInit.email;
    this.courses = objInit.courses;
    this.created = new Date(objInit.created);
}


function createPendingTutorsItem(objInit) {
    return new PendingTutorItem(objInit);
}

const getAllTutors = function () {
    let path = 'AdminTutor';
    return connectivityModule.http.get(path).then((tutors) => {
        let arrTutors = [];
        if (tutors.length > 0) {
            tutors.forEach(function (tutors) {
                arrTutors.push(createPendingTutorsItem(tutors));
            });
        }
        return Promise.resolve(arrTutors);
    }, (err) => {
        return Promise.reject(err);
    });
};

const aproveTutor = function (id) {
    let path = 'AdminTutor/approve';
    let idObj = {
        id: id
    };
    return connectivityModule.http.post(path, idObj).then(() => {
        return Promise.resolve(true);
    }, (err) => {
        return Promise.reject(err);
    });
};

const deleteTutor = function (id) {
    let path = 'AdminTutor/';
    return connectivityModule.http.delete(`${path}${id}`)
        .then((resp) => {
        console.log(resp, 'success deleted');
        return Promise.resolve(resp);
    }, (error) => {
        console.log(error, 'error deleted');
        return Promise.reject(error);
    });

};


export {
    getAllTutors,
    aproveTutor,
    deleteTutor
}