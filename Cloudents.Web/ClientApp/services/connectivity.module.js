import axios from "axios";
import qs from "query-string";

axios.defaults.paramsSerializer = params => qs.stringify(params, { indices: false });
axios.defaults.responseType = "json";
axios.defaults.baseURL = '/api';

export const connectivityModule = {
    http: {
        get: function(path, callback){
            if(callback){
                axios.get(path).then(function(data){
                    callback(data);
                },function(err){
                    callback(err, true);
                });
            }else{
                return axios.get(path);
            }
        },
        post: function(path, body, callback){
            if(callback){
                axios.post(path,body).then(function(data){
                    callback(data);
                },function(err){
                    callback(err, true);
                });
            }else{
                return axios.post(path,body);
            }
        },
        put: function(path, body, callback){
            if(callback){
                axios.put(path, body).then(function(data){
                    callback(data);
                },function(err){
                    callback(err, true);
                });
            }else{
                return axios.put(path, body);
            }
        },
        patch: function(path, body, callback){
            if(callback){
                axios.patch(path, body).then(function(data){
                    callback(data);
                },function(err){
                    callback(err, true);
                });
            }else{
                return axios.patch(path, body);
            }
        },
        delete: function(path, callback){
            if(callback){
                axios.delete(path).then(function(data){
                    callback(data);
                },function(err){
                    callback(err, true);
                });
            }else{
                return axios.delete(path);
            }
        }
    },

    ws: {
        
    }


}