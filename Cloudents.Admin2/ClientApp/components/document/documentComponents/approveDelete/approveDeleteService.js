import { connectivityModule } from '../../../../services/connectivity.module'

let url = 'AdminDocument';
function DocumentItem(ObjInit){
    this.id = ObjInit.id || '';
    this.preview = ObjInit.preview || '';
};
function createDocumentItem(ObjInit){
    return new DocumentItem(ObjInit)
}

export default {
    getDocuments: () => {
        return connectivityModule.http.get(`${url}`)
            .then((resp) => {
                console.log(resp, 'success get 20 docs');
                resp.forEach(function(doc) {
                    createDocumentItem(doc)
                });
                return Promise.resolve(resp)
            }, (error) => {
                console.log(error, 'error get 20 docs');
                return Promise.reject(error)
            })
    },
    deleteDocument: (id) => {
        return connectivityModule.http.delete(`${url}/${id}`)
            .then((resp) => {
                console.log(resp, 'delete success');
                return Promise.resolve(resp)
            }, (error) => {
                console.log(error, 'error delete');
                return Promise.reject(error)
            })
    },

    approveDocument: (arrIds) =>{
        return connectivityModule.http.post(`${url}`, arrIds)
            .then((resp) => {
                console.log(resp, 'post doc success');
                return Promise.resolve(resp)
            }, (error) => {
                console.log(error, 'error post doc');
                return Promise.reject(error)
            })
    }


}
