import {connectivityModule} from '../../../../services/connectivity.module'


function setIdQuery(ids){
    if(!ids) return "";
    let query = "";
    ids.forEach(function(id, index){
        query += index === 0 ? `?id=${id}` : `&id=${id}`;
    });
    return query;
}


const deleteDocument = function(ids) {
    let idsQueryString = setIdQuery(ids);
	const path = `AdminDocument` + idsQueryString;;
	return connectivityModule.http.delete(path).then((resp)=>{
        return Promise.resolve(resp);
    }, (err)=>{
        return Promise.reject(err);
    });
};

export {
    deleteDocument
}