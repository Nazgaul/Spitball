import { connectivityModule } from '../../../../services/connectivity.module'



function FlaggedDocumentItem(objInit){
    this.id = objInit.id || 'none';
    this.reason = objInit.reason || 'none';
    this.flaggedUserEmail = objInit.flaggedUserEmail || "none";
     this.preview = objInit.preview || "none";
    this.siteLink = objInit.siteLink || "none";
}

function createDocumentItem(objInit){
    return new FlaggedDocumentItem(objInit);
}

export default {
    getDocuments: () => {
        let url = 'AdminDocument/flagged';
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
        let url = 'AdminDocument';
        let ids = {'id': id};
        return connectivityModule.http.delete(url, ids)
            .then((resp) => {
                console.log(resp, 'delete success');
                return Promise.resolve(resp);
            }, (error) => {
                console.log(error, 'error delete');
                return Promise.reject(error);
            });
    },

    unflagDocument: (arrIds) =>{
        let url = 'AdminDocument/unFlag';
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
