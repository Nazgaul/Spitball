import axios from 'axios'
import qs from 'qs'

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
            axios.put(uri, body);
        },
        delete: function(path, ids){
            let uri = apiAddress + path;

            return axios.delete(uri,{
                params: ids,
                paramsSerializer: params => {
                  return qs.stringify(params, { arrayFormat: 'repeat' })
                }})
          
            .then(promiseReturn).catch(errorHandler);
         }
    }
};