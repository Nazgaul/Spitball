import { connectivityModule } from '../../../services/connectivity.module'

function cashoutUser(objInit) {
    this.userId = objInit.userId;
    this.cashOutPrice = objInit.cashOutPrice || 0;
    this.userEmail = objInit.userEmail || 'None';
    this.cashOutTime = objInit.cashOutTime || 0;
    this.fraudScore = objInit.fraudScore || 0;
    this.userQueryRatio = objInit.userQueryRatio;
    this.isSuspect = objInit.isSuspect || false;
    this.isIsrael = objInit.isIsrael || false;
    this.declinedReason = objInit.declinedReason || '';
    this.approved = objInit.approved || null;
    this.transactionId = objInit.transactionId || '';
}

function createCashoutItem(objInit) {
    return new cashoutUser(objInit);
}

const getCashoutList = function () {
    let path = "AdminUser/cashOut";
    return connectivityModule.http.get(path).then((cashoutList) => {
        let arrCashoutList = [];
        if (cashoutList.length > 0) {
            cashoutList.forEach((cashoutItem) => {
                arrCashoutList.push(createCashoutItem(cashoutItem))
            })
        }
        return Promise.resolve(arrCashoutList)
    }, (err) => {
        return Promise.reject(err)
    })
};

const declineCashout = function (transactionId, reason) {
    let path = "AdminUser/cashOut/decline";
    let data = {
        transactionId: transactionId,
        reason: reason
    };
    return connectivityModule.http.post(path, data).then((success) => {
        return Promise.resolve(success)
    }, (err) => {
        return Promise.reject(err)
    })
};

const approveCashout = function (transactionId) {
    let path = "AdminUser/cashOut/approve";
    let data = {
        transactionId: transactionId,
    };
    return connectivityModule.http.post(path, data).then((success) => {
        return Promise.resolve(success)
    }, (err) => {
        return Promise.reject(err)
    })
};


export {
    getCashoutList,
    createCashoutItem,
    declineCashout,
    approveCashout
}
