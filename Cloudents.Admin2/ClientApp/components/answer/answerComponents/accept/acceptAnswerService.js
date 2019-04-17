import {connectivityModule} from '../../../../services/connectivity.module'


    function setIdQuery(ids){
        if(!ids) return "";
        let arrIds = {
            ids:[]
        };
        ids.forEach(function(id){
            arrIds.ids.push(id);
        });
        return arrIds;
    }

    export const acceptAnswer =(ids)=>{
        let idsQueryString = setIdQuery(ids);
        let url = 'AdminAnswer/approve';
        return connectivityModule.http.post(`${url}`, idsQueryString)
        .then((resp)=>{
            console.log(resp, 'success deleted');
            return Promise.resolve(resp);
        }, (error)=>{
            console.log(error, 'error deleted');
            return Promise.reject(error);
        });
    };
