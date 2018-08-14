import axios from "axios";
import qs from "query-string";
import signalR from '@aspnet/signalr';

axios.defaults.paramsSerializer = params => qs.stringify(params, { indices: false });
axios.defaults.responseType = "json";
axios.defaults.baseURL = '/api';

const promiseReturn = function(data){
    return data;
}

const errorHandler = function(err){
    if(err.response.status === 401){
        window.location = '/signin';
    }else if(err.response.status === 404){
        window.location = '/error/notfound';
    }else{
        return err;
    }
}


export const connectivityModule = {
    http: {
        get: function(path, params="", callback){
            if(callback){
                axios.get(path, params).then(function(data){
                    callback(data);
                },function(err){
                    callback(err, true);
                });
            }else{
                return axios.get(path, params).then(promiseReturn, errorHandler)                
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
                return axios.post(path,body).then(promiseReturn, errorHandler)  
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
                return axios.put(path, body).then(promiseReturn, errorHandler)  
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
                return axios.patch(path, body).then(promiseReturn, errorHandler)
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
                return axios.delete(path).then(promiseReturn, errorHandler)
            }
        }
    },

    ws: {
        
    },

    //todo add error handler
    signalR: {
        getConnection: function(url){
            const connection = new signalR.HubConnectionBuilder()
            .withUrl(url)
            .build();
            return connection;
        },
        on: function(connection){
            return connection.on;
        },
        invoke: function(connection, messageString, messageObj){
            return connection.invoke(messageString, messageObj);
        },
        start: function(connections){
            if(!!connections && connections.length > 0){
                connections.forEach(connection => {
                    connection.start();
                })
            }
        }
    }


}