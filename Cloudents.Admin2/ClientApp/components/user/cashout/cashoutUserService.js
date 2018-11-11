import {connectivityModule} from '../../../services/connectivity.module'

function cashoutUser(objInit){
    this.userId = objInit.userId;
    this.cashOutPrice = objInit.cashOutPrice;
    this.userEmail = objInit.userEmail;
    this.cashOutTime = objInit.cashOutTime;
    this.fraudScore = objInit.fraudScore;
    this.userQueryRatio = objInit.userQueryRatio;
    this.isSuspect = objInit.isSuspect;
    this.isIsrael = objInit.isIsrael;
}

function createCashoutItem(objInit){
    return new cashoutUser(objInit);
}

const getCashoutList = function(){
    let path = "AdminUser/cashOut";
    return connectivityModule.http.get(path).then((cashoutList)=>{
        let arrCashoutList = [];
        if(cashoutList.length > 0){
            cashoutList.forEach((cashoutItem)=>{
                arrCashoutList.push(createCashoutItem(cashoutItem))
            })
        }
        return Promise.resolve(arrCashoutList)
    }, (err)=>{
        return Promise.reject(err)
    })
}


export{
    getCashoutList
}
