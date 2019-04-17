import { connectivityModule } from '../../../services/connectivity.module'

const deleteUser = function(userId){
    let path = `AdminUser/${userId}`;
    return connectivityModule.http.delete(path).then(()=>{
        return Promise.resolve(true);
    },(err)=>{
        return Promise.reject(err);
    });
};

export{
    deleteUser,
}