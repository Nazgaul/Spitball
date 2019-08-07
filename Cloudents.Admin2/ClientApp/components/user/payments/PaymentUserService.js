﻿import { connectivityModule } from '../../../services/connectivity.module';


function PaymentRequestItem(objInit) {
    this.price = objInit.price.toFixed(2);
    //this.sellerKey = objInit.sellerKey;
    //this.paymentKey = objInit.paymentKey;
    this.tutorId = objInit.tutorId;
    this.tutorName = objInit.tutorName;
    this.userId = objInit.userId;
    this.userName = objInit.userName;
    this.tutorPayme = objInit.cantPay || true; 
    this.studyRoomSessionId = objInit.studyRoomSessionId;
    this.created = new Date(objInit.created).toLocaleString();
    this.duration = objInit.duration;
    this.totalPrice = this.price*this.duration/60;
    this.subsidizing = (subsidizingPrice(this.price)*this.duration/60).toFixed(2);
}
function createPaymentRequestItem(objInit) {
    return new PaymentRequestItem(objInit);
}

const subsidizingPrice = function(price) {
    if (price < 55)
    {
        return price;
    }

    var subsidizingPrice = price - 70;
    if (subsidizingPrice < 55)
    {
        return 55;
    }

    return subsidizingPrice;
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


const approvePayment = function (item,spitballPay) {
    return connectivityModule.http.post(`${path}`, {
       studentPay : item.subsidizing,
       spitballPay: spitballPay,
       userId: item.userId,
       tutorId: item.tutorId,
       StudyRoomSessionId: item.studyRoomSessionId
    });
};

const declinePayment = function (item) {
    return connectivityModule.http.delete(`${path}?StudyRoomSessionId=${item.studyRoomSessionId}`);
};

export {
    getPaymentRequests,
    approvePayment,
    subsidizingPrice,
    declinePayment
};