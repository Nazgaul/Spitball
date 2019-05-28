import { connectivityModule } from '../../../services/connectivity.module'


function PendingTutorItem(objInit) {
    this.id = objInit.id;
    this.firstName = objInit.firstName;
    this.lastName = objInit.lastName;
    this.bio = objInit.bio;
    this.price = objInit.price;
    this.email = objInit.email;
    objInit.courses = objInit.courses || [];
    this.courses =  objInit.courses.split(",");
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
        return arrTutors;
    });
};

const aproveTutor = function (id) {
    let path = 'AdminTutor/approve';
    let idObj = {
        id: id
    };
    return connectivityModule.http.post(path, idObj).then(() => {
        return true;
    });
};

const deleteTutor = function (id) {
    let path = 'AdminTutor/';
    return connectivityModule.http.delete(`${path}${id}`);
};


export {
    getAllTutors,
    aproveTutor,
    deleteTutor
}