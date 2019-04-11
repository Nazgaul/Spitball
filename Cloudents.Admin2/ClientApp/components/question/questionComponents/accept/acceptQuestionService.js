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

    export const acceptQuestion =(ids)=>{
        let idsQueryString = setIdQuery(ids);
        let url = 'AdminQuestion/approve';
        return connectivityModule.http.post(`${url}`, idsQueryString)
        .then((resp)=>{
            console.log(resp, 'success accepted');
            return Promise.resolve(resp);
        }, (error)=>{
            console.log(error, 'error accepted');
            return Promise.reject(error);
        });
    };
