import {connectivityModule} from '../../../../services/connectivity.module'


    function setIdQuery(ids){
        if(!ids) return "";
        let query = "";
        ids.forEach(function(id, index){
            query += index === 0 ? `?id=${id}` : `&id=${id}`;
        });
        return query;
    }

    export const deleteQuestion =(ids)=>{
        let idsQueryString = setIdQuery(ids);
        let url = 'AdminQuestion' + idsQueryString;
        return connectivityModule.http.delete(`${url}`)
        .then((resp)=>{
            console.log(resp, 'success deleted');
            return Promise.resolve(resp);
        }, (error)=>{
            console.log(error, 'error deleted');
            return Promise.reject(error);
        });
    };
