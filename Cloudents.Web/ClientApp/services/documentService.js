import { connectivityModule } from "./connectivity.module"
import searchService from './searchService'

function documentUserItem(ObjInit){
    this.name = ObjInit.uploaderName
    this.userId = ObjInit.uploaderId
    this.isTutor = false;
}

function DocumentItem(ObjInit) {
    this.name = ObjInit.name;
    this.date = ObjInit.date;
    this.course = ObjInit.course;
    this.id = ObjInit.id;
    this.university = ObjInit.university || '';
    this.user = !!ObjInit.user ? searchService.createTutorItem(ObjInit.user) : new documentUserItem(ObjInit);
    this.views = ObjInit.views || 0;
    this.pages = ObjInit.pages || 0;
    this.docType = ObjInit.type || '';
    this.isPlaceholder = ObjInit.isPlaceholder || false;
    this.professor = ObjInit.professor || '';
    this.price = ObjInit.price || 0;
    this.isPurchased = ObjInit.isPurchased || false;
    this.uploaderName = ObjInit.uploaderName;
};

function createDocumentItem(ObjInit) {
    return new DocumentItem(ObjInit)
}

function createDocumentPreview(itemPreview){
    if (!itemPreview || itemPreview.length === 0) {
        let location = `${global.location.origin}/images/doc-preview-empty.png`;
        itemPreview.push(location);
    }

    return itemPreview;
}

function createDocumentContentType(itemPreview){
    let arrPreview = createDocumentPreview(itemPreview);
    let postfix = arrPreview[0].split('?')[0].split('.');
    return postfix[postfix.length - 1];
}
function createVideoPreview(objInit){
    return new VideoPreview(objInit)
}
function VideoPreview(objInit){
    this.locator = objInit.locator;
    this.poster = objInit.poster;
}
function DocumentObject(objInit){
    this.details = createDocumentItem(objInit.details);
    this.preview = objInit.details.documentType === 'Video'? createVideoPreview(objInit.preview):createDocumentPreview(objInit.preview);
    this.content = objInit.content || '';
    this.contentType = objInit.details.documentType === 'Video'? '': createDocumentContentType(objInit.preview);
    this.documentType = objInit.details.documentType || '';
}

function createDocumentObj(ObjInit) {
    return new DocumentObject(ObjInit)
}

function getDocument(id){
    return connectivityModule.http.get(`/Document/${id}`).then(({data})=>{
        return createDocumentObj(data);
    });

}

export default {
    sendDocumentData: (data) => connectivityModule.http.post("/Document", data),
    deleteDoc: (id) => connectivityModule.http.delete(`/Document/${id}`),
    purchaseDocument: (id) => connectivityModule.http.post("/Document/purchase", {id}),
    changeDocumentPrice: (data) => connectivityModule.http.post("/Document/price", data),
    getDocument,
    createDocumentItem
}