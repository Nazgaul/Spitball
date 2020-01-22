import { connectivityModule } from '../../../services/connectivity.module'

function CashoutUser(objInit) {
    this.userId = objInit.userId;
    this.awardCount = objInit.awardCount || 0;
    this.cashOutPrice = objInit.cashOutPrice || 0;
    this.buyCount = objInit.buyCount || 0;
    this.cashOut= objInit.cashOut || 0;
    this.userEmail = objInit.userEmail || 'None';
    this.cashOutTime = new Date( objInit.cashOutTime );
    this.approved = objInit.approved || null;
    this.transactionId = objInit.transactionId || '';
    this.correctAnswer = objInit.correctAnswer;
    this.declinedReason = objInit.declinedReason;
    this.deletedCorrectAnswer = objInit.deletedCorrectAnswer;
    this.country = objInit.country;
    this.referCount = objInit.referCount;
    this.soldDeletedDocument = objInit.soldDeletedDocument;
    this.soldDocument = objInit.soldDocument;
}

function createCashoutItem(objInit) {
    return new CashoutUser(objInit);
}

const getCashoutList = function () {
    let path = "AdminUser/cashOut";
    return connectivityModule.http.get(path).then((cashoutList) => {
        let arrCashoutList = [];
        if (cashoutList.length > 0) {
            cashoutList.forEach((cashoutItem) => {
                arrCashoutList.push(createCashoutItem(cashoutItem));
            });
        }
        return Promise.resolve(arrCashoutList);
    }, (err) => {
        return Promise.reject(err);
    });
};

const declineCashout = function (transactionId, reason) {
    let path = "AdminUser/cashOut/decline";
    let data = {
        transactionId: transactionId,
        reason: reason
    };
    return connectivityModule.http.post(path, data).then((success) => {
        return Promise.resolve(success);
    }, (err) => {
        return Promise.reject(err);
    });
};

const approveCashout = function (transactionId) {
    let path = "AdminUser/cashOut/approve";
    let data = {
        transactionId: transactionId
    };
    return connectivityModule.http.post(path, data).then((success) => {
        return Promise.resolve(success);
    }, (err) => {
        return Promise.reject(err);
    });
};


export {
    getCashoutList,
    createCashoutItem,
    declineCashout,
    approveCashout
}
