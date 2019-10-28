import {connectivityModule} from './connectivity.module';

function TutorPaymentSession(objInit) {
    this.tutorId = objInit.id;
    this.name = objInit.name;
    this.phoneNumber = objInit.phoneNumber;
    this.email = objInit.email;
    this.totalHours = objInit.totalHours;
    this.totalStudents = objInit.totalStudents;
    this.price = objInit.price;
    this.needToPay = objInit.needToPay;
}

function createTutorPaymentSession(objInit) {
    return new TutorPaymentSession(objInit);
}

function getTutorPaymentSession() {
    return connectivityModule.http.get('AdminStudyRoom/tutors').then(tutorRes => {
        return tutorRes.map(tutor => createTutorPaymentSession(tutor));
    });
}


function TutorPaymentBills(objInit) {
    this.name = objInit.name;
    this.email = objInit.email;
    this.created = objInit.created;
    this.ended = objInit.ended;
    this.minutes = objInit.minutes;
    this.cost = objInit.cost;
    this.isPayed = objInit.isPayed;
}

function createTutorPaymentBill(objInit) {
    return new TutorPaymentBills(objInit);
}

function getTutorPaymentBills(id) {
    return connectivityModule.http.get(`AdminStudyRoom/bills?id=${id}`).then(billsRes => {
        return billsRes.map(bill => createTutorPaymentBill(bill));
    });
}

export default {
    getTutorPaymentSession,
    getTutorPaymentBills
}