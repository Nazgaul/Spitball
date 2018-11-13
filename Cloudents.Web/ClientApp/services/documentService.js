import { connectivityModule } from "./connectivity.module"

function DocumentItem(ObjInit){
    this.name = ObjInit.name || '';
    this.date = ObjInit.date || '';
    this.course = ObjInit.course || '';
    this.id= ObjInit.id || '';
    this.university= ObjInit.university || '';
    this.user = ObjInit.user || {};
    this.views = ObjInit.views || 0;
    this.pages = ObjInit.pages || '';
    this.extension = ObjInit.extension || '';
    this.docType = ObjInit.type || '';
    this.isPlaceholder = ObjInit.isPlaceholder || false;
    this.professor = ObjInit.professor || '';
};
function createDocumentItem(ObjInit){
    return new DocumentItem(ObjInit)
}

export default {
    sendDocumentData: (data) => connectivityModule.http.post("/Document", data),
    getDocument : (id) => connectivityModule.http.get(`/Document/${id}`),
    createDocumentItem
}