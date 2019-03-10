import Twilio, { LocalDataTrack } from 'twilio-video';
import { connectivityModule } from '../../services/connectivity.module';
// import whiteBoardService from "./whiteboard/whiteBoardService";


const dataTrack = new LocalDataTrack();

const uploadCanvasImage = function(formData){
    return connectivityModule.http.post("Tutoring/upload", formData);
};
const getSharedDoc = async function(docName){
   return connectivityModule.http.post("Tutoring/document", docName)
       .then((resp)=>{
        return  resp.data.link
   })
};
const passSharedDocLink = function (docUrl) {
    let transferDataObj = {
        type: "sharedDocumentLink",
        data: docUrl
    };
    let normalizedData = JSON.stringify(transferDataObj);
    console.log('service data track', dataTrack);
    dataTrack.send(normalizedData);
}


export {
    dataTrack,
    uploadCanvasImage,
    getSharedDoc,
    passSharedDocLink
}