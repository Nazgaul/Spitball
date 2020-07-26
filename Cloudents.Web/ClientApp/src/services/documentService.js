import axios from 'axios'

const documentInstance = axios.create({
    baseURL:'/api/Document'
})

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
    this.documentType = objInit.details.documentType || '';
    this.preview = this.documentType === 'Video'? createVideoPreview(objInit.preview):createDocumentPreview(objInit.preview);
    this.date = objInit.details.dateTime;
    this.id = objInit.details.id;
    this.isPurchased = objInit.details.isPurchased;
    this.pages = objInit.details.pages || 0;
    this.price = objInit.details.price || 0;
    this.priceType = objInit.details.priceType;
    this.userId = objInit.details.userId;
    this.userName = objInit.details.userName;
    this.title = objInit.details.title;
}

export default {
    // async voteDocument(id, voteType){
    //     return await documentInstance.post('vote',{id,voteType})
    // },
    async changeDocumentName(data){ 
        return await documentInstance.post('rename',data)
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
    async getDocument(id){ 
        let {data} = await documentInstance.get(`${id}`)

        return new DocumentObject(data);
    },
}