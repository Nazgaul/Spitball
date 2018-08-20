import axios from "axios";
import qs from "query-string";
import signalR from '@aspnet/signalr';
import analyticsService from './analytics.service'

axios.defaults.paramsSerializer = params => qs.stringify(params, { indices: false });
axios.defaults.responseType = "json";
axios.defaults.baseURL = '/api';


const promiseReturn = function(data){
    // "this" is bound to the timerObject
    let endTime = new Date().getTime();
    analyticsService.sb_fireTimingAnalytic(this.requestMethod, this.path, endTime - this.startTime, "SUCCESS")

    return data;
}

const errorHandler = function(err){
    // "this" is bound to the timerObject
    let endTime = new Date().getTime();
    analyticsService.sb_fireTimingAnalytic(this.requestMethod, this.path, endTime - this.startTime, "ERROR")
    
    if(err.response.status === 401){
        window.location = '/signin';
    }else if(err.response.status === 404){
        window.location = '/error/notfound';
    }else{
        return err;
    }
}

const timerObject = function(path, requestMethod){
    this.startTime = new Date().getTime();
    this.path = path;
    this.requestMethod = requestMethod;
}

export const connectivityModule = {
    http: {
        get: function(path, params="", callback){
            let timeProps = new timerObject(path, 'GET');
            if(callback){
                axios.get(path, params).then(function(data){
                    callback(data);
                },function(err){
                    callback(err, true);
                });
            }else{        
                return axios.get(path, params).then(promiseReturn.bind(timeProps), errorHandler.bind(timeProps))                
            }
        },
        post: function(path, body, callback){
            let timeProps = new timerObject(path, 'POST');
            if(callback){
                axios.post(path,body).then(function(data){
                    callback(data);
                },function(err){
                    callback(err, true);
                });
            }else{
                return axios.post(path,body).then(promiseReturn.bind(timeProps), errorHandler.bind(timeProps))  
            }
        },
        put: function(path, body, callback){
            let timeProps = new timerObject(path, 'PUT');
            if(callback){
                axios.put(path, body).then(function(data){
                    callback(data);
                },function(err){
                    callback(err, true);
                });
            }else{
                return axios.put(path, body).then(promiseReturn.bind(timeProps), errorHandler.bind(timeProps))  
            }
        },
        patch: function(path, body, callback){
            let timeProps = new timerObject(path, 'PATCH');
            if(callback){
                axios.patch(path, body).then(function(data){
                    callback(data);
                },function(err){
                    callback(err, true);
                });
            }else{
                return axios.patch(path, body).then(promiseReturn.bind(timeProps), errorHandler.bind(timeProps))
            }
        },
        delete: function(path, callback){
            let timeProps = new timerObject(path, 'DELETE');
            if(callback){
                axios.delete(path).then(function(data){
                    callback(data);
                },function(err){
                    callback(err, true);
                });
            }else{
                return axios.delete(path).then(promiseReturn.bind(timeProps), errorHandler.bind(timeProps))
            }
        }
    },

    ws: {
        
    },

    //todo add error handler
    signalR: {
        createConnection: function(url){
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