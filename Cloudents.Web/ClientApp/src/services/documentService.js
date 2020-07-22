import axios from 'axios'
import {Item} from './Dto/item.js';
import searchService from './searchService'

const documentInstance = axios.create({
    baseURL:'/api/Document'
})

function documentUserItem(ObjInit){
    this.name = ObjInit.uploaderName;
    this.userId = ObjInit.uploaderId;
    this.isTutor = false;
}

function DocumentItem(obj) {
    let ObjInit = obj.document;
    let ObjInitTutor = obj.tutor;
    this.name = ObjInit.name;
    this.date = ObjInit.date;
    this.course = ObjInit.course;
    this.id = ObjInit.id;
    this.user = !!ObjInit.user ? searchService.createTutorItem(ObjInit.user) : new documentUserItem(ObjInit);
    //this.views = ObjInit.views || 0;
    this.pages = ObjInit.pages || 0;
    this.price = ObjInit.price || 0;
    this.priceType = ObjInit.priceType;
    this.isPurchased = obj.isPurchased || false;
    this.uploaderName = ObjInit.uploaderName;
    this.tutor = ObjInitTutor ? searchService.createTutorItem(ObjInitTutor) : null;
    this.feedItem = new Item[ObjInit.documentType](ObjInit);
}

function createDocumentItem(ObjInit) {
    return new DocumentItem(ObjInit);
}

// TODO do it with skeleton!
function createDocumentPreview(itemPreview){
    if (!itemPreview || itemPreview.length === 0) {
        let location = require("../components/images/doc-preview-empty.png");
        itemPreview.push(location);
    }

    return itemPreview;
}

function createVideoPreview(objInit){
    return new VideoPreview(objInit);
}
function VideoPreview(objInit){
    this.locator = objInit.locator;
    this.poster = objInit.poster;
}
function DocumentObject(objInit){
    this.details = createDocumentItem(objInit.details);
    this.content = objInit.content;
    this.preview = objInit.details.document.documentType === 'Video'? createVideoPreview(objInit.preview):createDocumentPreview(objInit.preview);
    this.documentType = objInit.details.document.documentType || '';
}

//function createDocumentObj(ObjInit) {
    //return new DocumentObject(ObjInit);
//}
export default {
    // async voteDocument(id, voteType){
    //     return await documentInstance.post('vote',{id,voteType})
    // },
    async changeDocumentName(data){ 
        return await documentInstance.post('rename',data)
    },
    async changeDocumentPrice(data){ 
        return await documentInstance.post('price',data)
    },
    async deleteDoc(id){ 
        return await documentInstance.delete(`${id}`)
    },
    async sendDocumentData(data){ 
        return await documentInstance.post('',data)
    },
    async purchaseDocument(id){ 
        return await documentInstance.post('purchase',{id})
    },
    async getStudyDocuments(params){ 
        let {data} = await documentInstance.get('similar',{params})
        return data.map(doc=> new Item[doc.documentType](doc))
    },
    async getDocument(id){ 
        let {data} = await documentInstance.get(`${id}`)

        return new DocumentObject(data);
    },
}