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
    this.date = objInit.details.dateTime; // search and destroy
    this.id = objInit.details.id;
    this.isPurchased = objInit.details.isPurchased;
    this.pages = objInit.details.pages || 0;
    this.price = objInit.details.price || 0;
    // this.price = {
        //     amount: objInit.price?.amount,
        //     currency: objInit.price?.currency
        // }
    this.subscriptionPrice = {
        amount: objInit.details.subscriptionPrice?.amount,
        currency: objInit.details.subscriptionPrice?.currency
    }
    this.courseId = objInit.details.courseId;

    this.userId = objInit.details.userId;
    this.userName = objInit.details.userName;
    this.title = objInit.details.title;
}

export default {
    // async deleteDoc(id){ 
    //    return await documentInstance.delete(`${id}`)
    // },
    async getDocument(id){ 
        let {data} = await documentInstance.get(`${id}`)

        return new DocumentObject(data);
    },
}