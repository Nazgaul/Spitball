import axios from 'axios'

const promiseReturn = function(response){
    return response.data;
};
const errorHandler = function(err){
    if (err.response.status == 403) {
        alert("You don't have access to this");
    }
    if (err.response.status == 401) {
        window.location.reload();
    }
    return Promise.reject(err);
};

const apiAddress = `${window.location.origin}/api/`;

export const connectivityModule = {
    http:{
        get: function(path, params){
            let uri = apiAddress + path;
            return axios.get(uri, params).then(promiseReturn).catch(errorHandler);
        },
        post: function(path, body){
            let uri = apiAddress + path;
            return axios.post(uri,body).then(promiseReturn).catch(errorHandler);
        },
        put: function(path, body){
            let uri = apiAddress + path;
            return axios.put(uri, body).then(promiseReturn).catch(errorHandler);
        },
        delete: function(path, ids){
            let uri = apiAddress + path;
            return axios.delete(uri, ids)
          
            .then(promiseReturn).catch(errorHandler);
        }
    }
};