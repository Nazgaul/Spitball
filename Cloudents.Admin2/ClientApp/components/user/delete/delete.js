import { connectivityModule } from '../../../services/connectivity.module'




function deleteUser(id) {
    return connectivityModule.http.delete("AdminUser/" + id,);
}

export default {
    deleteUser
}