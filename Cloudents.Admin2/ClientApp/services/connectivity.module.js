import axios from 'axios'

const promiseReturn = function(response){
    return response.data;
}
const errorHandler = function(err){
    return Promise.reject(err);
}

const apiAdress = `${window.location.origin}/api/`

export const connectivityModule = {
    http:{
        get: function(path, params){
            let uri = apiAdress + path;
            return axios.get(uri, params).then(promiseReturn).catch(errorHandler)
        },
        post: function(path, body){
            let uri = apiAdress + path;
            return axios.post(uri,body).then(promiseReturn).catch(errorHandler)
        },
        put: function(path, body){
            let uri = apiAdress + path;
            return axios.put(uri, body).then(promiseReturn).catch(errorHandler)
        },
        delete: function(path){
            let uri = apiAdress + path;
            return axios.delete(uri).then(promiseReturn).catch(errorHandler)
        }
    }
}