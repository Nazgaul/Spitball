import { connectivityModule } from '../../../services/connectivity.module'

function ServerUserObj(objInit){
    this.id = objInit.id;
    this.country = objInit.country;
}

const setUserCountry = function(userObj){
    let path = "AdminUser/country";
    let toSend = new ServerUserObj(userObj);
    return connectivityModule.http.post(path, toSend).then(()=>{
        return Promise.resolve(true);
    },(err)=>{
        return Promise.reject(err);
    });
};

export{
    setUserCountry,
}