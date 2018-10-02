import {connectivityModule} from '../../../services/connectivity.module'

const suspendUser = function(id, deleteUserQuestions){
    let path = "AdminUser/suspend";
    let data = {
        id,
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