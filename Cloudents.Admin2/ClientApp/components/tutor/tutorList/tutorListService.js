import { connectivityModule } from '../../../services/connectivity.module';


function TutorItem(objInit) {
    this.id = objInit.id;
    this.name = objInit.name;
    this.email = objInit.email;
    this.phoneNumber = objInit.phoneNumber;
    this.country = objInit.country;
    this.state = objInit.state;
}
function createTutorItem(objInit) {
    return new TutorItem(objInit);
}


const path = 'AdminTutor/';



const getTutorList = function (country, state, filter) {
    country = country === 'All' ? '' : country;
    state = state === 'All' ? null : state;
    let query = `?Country=${country}`;

    if(state) {
        query += `&State=${state}`
    }
    if(filter){
        query += `&Term=${filter}`;
    }
        return connectivityModule.http.get(`${path}search${query}`)
        .then((tutorList) => {
            let arrTutorList = [];
            if (tutorList.length > 0) {
                tutorList.forEach((tutor) => {
                    arrTutorList.push(createTutorItem(tutor));
                });
            }
            return Promise.resolve(arrTutorList);
        }, (err) => {
            return Promise.reject(err);
        });

};
export {
    getTutorList
};