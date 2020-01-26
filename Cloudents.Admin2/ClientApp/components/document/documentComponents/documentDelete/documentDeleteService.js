import {connectivityModule} from '../../../../services/connectivity.module'

const deleteDocument = function(id) {
    const path = `AdminDocument`;   
    let ids = {'id': id};
	return connectivityModule.http.delete(path,ids).then((resp)=>{
        return Promise.resolve(resp);
    }, (err)=>{
        return Promise.reject(err);
    });
};

export {
    deleteDocument
}