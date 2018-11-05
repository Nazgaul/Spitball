import { connectivityModule } from "./connectivity.module"

function DocumentItem(ObjInit){
    this.course = ObjInit.course || '';
    this.id= ObjInit.id || '';
    this.title= ObjInit.title || '';
    this.university= ObjInit.university || '';
    this.user = ObjInit.user || {};
    this.views= ObjInit.views || '';
};
function createDocumentItem(ObjInit){
    return new DocumentItem(ObjInit)
}

export default {
    sendDocumentData: (data) => connectivityModule.http.post("/Document", data),
    getDocument : (id) => connectivityModule.http.get(`/Document/${id}`)
        .then((re)=>{
           console.log('rererere', re)
        }),
    createDocumentItem
}