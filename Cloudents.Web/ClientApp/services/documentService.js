import { connectivityModule } from "./connectivity.module"

function DocumentItem(ObjInit) {
    this.name = ObjInit.name || '';
    this.date = ObjInit.date || '';
    this.course = ObjInit.course || '';
    this.id = ObjInit.id || '';
    this.university = ObjInit.university || '';
    this.user = ObjInit.user || {};
    this.views = ObjInit.views || 0;
    this.downloads = ObjInit.downloads || 0;
    this.pages = ObjInit.pages || '';
    this.extension = ObjInit.extension || '';
    this.docType = ObjInit.type || '';
    this.isPlaceholder = ObjInit.isPlaceholder || false;
    this.professor = ObjInit.professor || '';
    this.price = ObjInit.price || 0;
    this.isPurchased = ObjInit.isPurchased || false;
};

function createDocumentItem(ObjInit) {
    return new DocumentItem(ObjInit)
}

export default {
    sendDocumentData: (data) => connectivityModule.http.post("/Document", data),
    purchaseDocument: (id) => connectivityModule.http.post("/Document/purchase", {id}),
    getDocument: (id) => connectivityModule.http.get(`/Document/${id}`),
    createDocumentItem
}