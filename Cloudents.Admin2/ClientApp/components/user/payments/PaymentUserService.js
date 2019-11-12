import { connectivityModule } from '../../../services/connectivity.module';

function PaymentRequestItem(objInit) {
    this.price = objInit.price.toFixed(2);
    //this.sellerKey = objInit.sellerKey;
    //this.paymentKey = objInit.paymentKey;
    this.tutorId = objInit.tutorId;
    this.tutorName = objInit.tutorName;
    this.userId = objInit.userId;
    this.userName = objInit.userName;
    this.tutorPayme = objInit.cantPay; 
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
    return price;
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
        return arrPaymentRequestsList;
    }, (err) => {
        return Promise.reject(err);
    });
};


const approvePayment = function (item) {
    return connectivityModule.http.post(`${path}`, item);
};

const declinePayment = function (item) {
    return connectivityModule.http.delete(`${path}?StudyRoomSessionId=${item.studyRoomSessionId}`);
};

function UserSessionPayment(objInit) {
    this.cantPay = objInit.cantPay;
    this.couponCode = objInit.couponCode || null;
    this.couponType = objInit.couponType || null;
    this.couponValue = objInit.couponValue || null;
    this.created = objInit.created;
    this.duration = objInit.duration;
    this.spitballPayPerHour = objInit.spitballPayPerHour;
    this.studentPayPerHour = objInit.studentPayPerHour || objInit.tutorPricePerHour;
    this.studyRoomSessionId = objInit.studyRoomSessionId;
    this.tutorId = objInit.tutorId;
    this.tutorName = objInit.tutorName;
    this.tutorPricePerHour = objInit.tutorPricePerHour;
    this.userId = objInit.userId;
    this.userName = objInit.userName;
}

function createUserSessionPayment(data) {
    return new UserSessionPayment(data)
}

function handleError(err) {
    return err;
}

function getUserSessionPayment(id) {
    return connectivityModule.http.get(`${path}${id}`).then(createUserSessionPayment).catch(handleError)
}

export {
    getPaymentRequests,
    getUserSessionPayment,
    approvePayment,
    subsidizingPrice,
    declinePayment
};