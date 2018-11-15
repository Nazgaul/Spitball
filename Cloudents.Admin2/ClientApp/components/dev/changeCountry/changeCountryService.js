import { connectivityModule } from '../../../services/connectivity.module'

function serverUserObj(objInit){
    this.id = objInit.id;
    this.country = objInit.country;
}

const setUserCountry = function(userObj){
    let path = "AdminUser/country"
    let toSend = new serverUserObj(userObj);
    return connectivityModule.http.post(path, toSend).then(()=>{
        return Promise.resolve(true)
    },(err)=>{
        return Promise.reject(err)
    })
}

export{
    setUserCountry,
}