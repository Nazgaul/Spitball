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
                resp.documents.forEach(function(doc) {
                   return createDocumentItem(doc);
                });
                return resp;
            });
    },
    deleteDocument: (id) => {
        let ids = {'id': id};
        return connectivityModule.http.delete(url, ids);
            
    },

    approveDocument: (arrIds) =>{
        return connectivityModule.http.post(`${url}`, {"id": arrIds});
           
    }


}
