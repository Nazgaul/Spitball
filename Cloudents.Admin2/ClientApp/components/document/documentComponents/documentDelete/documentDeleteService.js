import {connectivityModule} from '../../../../services/connectivity.module'

const deleteDocument = function(id) {
	const path = `AdminDocument/${id}`;
	return connectivityModule.http.delete(path).then((resp)=>{
        return Promise.resolve(resp);
    }, (err)=>{
        return Promise.reject(err);
    });
};

export {
    deleteDocument
}