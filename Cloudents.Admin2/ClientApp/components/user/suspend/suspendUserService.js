import {connectivityModule} from '../../../services/connectivity.module'

const suspendUser = function(ids, reason){
    let path = "AdminUser/suspend";
    let data = {
        ids,
        reason
    };
    return connectivityModule.http.post(path, data).then((resp)=>{
        return Promise.resolve(resp.email);
    }, (err)=>{
        return Promise.reject(err);
    });
};
const releaseUser = function(ids){
    let path = "AdminUser/unSuspend";
    let data = {
        ids
    };
    return connectivityModule.http.post(path, data).then((resp)=>{
        return Promise.resolve(resp.email);
    }, (err)=>{
        return Promise.reject(err);
    });
};

export {
    suspendUser,
    releaseUser
}