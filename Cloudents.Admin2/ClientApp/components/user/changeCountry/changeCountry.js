import { connectivityModule } from '../../../services/connectivity.module'

const path = "AdminUser/country";


function updateCountry(userObj) {
    return connectivityModule.http.post(path, userObj).then(res => {
        return res;
    });
}

export default {
    updateCountry
}