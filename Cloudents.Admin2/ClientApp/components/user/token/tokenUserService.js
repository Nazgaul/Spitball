import {connectivityModule} from '../../../services/connectivity.module'

const grantTokens = function(userId, tokens, transactionType){
    let path = "AdminUser/sendTokens";
    let data = {
        userId,
        tokens,
        transactionType
    };
    return connectivityModule.http.post(path, data).then(()=>{
        return Promise.resolve();
    }, (err)=>{
        return Promise.reject(err);
    });
};

export {
    grantTokens
}