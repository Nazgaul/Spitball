import {connectivityModule} from '../../../services/connectivity.module'

const suspendUser = function(ids, deleteUserQuestions){
    let path = "AdminUser/suspend";
    let data = {
        ids,
        deleteUserQuestions
    }
    return connectivityModule.http.post(path, data).then((resp)=>{
        return Promise.resolve(resp.email);
    }, (err)=>{
        return Promise.reject(err);
    })
}

export {
    suspendUser
}