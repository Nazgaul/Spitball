import { connectivityModule } from '../../../../services/connectivity.module'

let url = 'AdminDocument';
function DocumentItem(objInit){
    this.id = objInit.id || '';
    this.preview = objInit.preview || '';
    this.url = objInit.siteLink || '';
};
function createDocumentItem(objInit){
    return new DocumentItem(objInit);
}

export default {
    getDocuments: () => {
        return connectivityModule.http.get(`${url}`)
            .then((resp) => {
                console.log(resp, 'success get 20 docs');
                resp.forEach(function(doc) {
                   return createDocumentItem(doc);
                });
                return Promise.resolve(resp);
            }, (error) => {
                console.log(error, 'error get 20 docs');
                return Promise.reject(error);
            });
    },
    deleteDocument: (id) => {
        return connectivityModule.http.delete(`${url}/${id}`)
            .then((resp) => {
                console.log(resp, 'delete success');
                return Promise.resolve(resp);
            }, (error) => {
                console.log(error, 'error delete');
                return Promise.reject(error);
            });
    },

    approveDocument: (arrIds) =>{
        return connectivityModule.http.post(`${url}`, {"id": arrIds})
            .then((resp) => {
                console.log(resp, 'post doc success');
                return Promise.resolve(resp);
            }, (error) => {
                console.log(error, 'error post doc');
                return Promise.reject(error);
            });
    }


}
