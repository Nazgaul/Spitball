﻿import { connectivityModule } from '../../../services/connectivity.module';


function PaymentRequestItem(objInit) {
    this.price = objInit.price.toFixed(2);
    this.sellerKey = objInit.sellerKey;
    this.paymentKey = objInit.paymentKey;
    this.tutorId = objInit.tutorId;
    this.tutorName = objInit.tutorName;
    this.userId = objInit.userId;
    this.userName = objInit.userName;
    this.studyRoomSessionId = objInit.studyRoomSessionId;
    this.created = new Date(objInit.created).toLocaleString();
    this.duration = objInit.duration;
    this.subsidizing = objInit.subsidizing.toFixed(2);
}
function createPaymentRequestItem(objInit) {
    return new PaymentRequestItem(objInit);
}

const path = 'AdminPayment/';

const getPaymentRequests = function () {
    return connectivityModule.http.get(`${path}`).then((newPaymentRequestsList) => {
        let arrPaymentRequestsList = [];
        if (newPaymentRequestsList.length > 0) {
            newPaymentRequestsList.forEach((pr) => {
                arrPaymentRequestsList.push(createPaymentRequestItem(pr));
            });
        }
        return Promise.resolve(arrPaymentRequestsList);
    }, (err) => {
        return Promise.reject(err);
    });
};


const approvePayment = function (item) {
    return connectivityModule.http.post(`${path}`, {
        "UserKey": item.paymentKey,
        "TutorKey": item.sellerKey,
        "Amount": item.price,
        "StudyRoomSessionId": item.studyRoomSessionId
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
    getPaymentRequests,
    approvePayment
};