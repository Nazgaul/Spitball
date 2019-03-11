import { dataTrack } from '../tutorService';
import { connectivityModule } from "../../../services/connectivity.module";

function MessageItem(objInit) {
    this.type = 'tutoringChatMessage';
    this.data = {
        "text":  objInit.text || '',
        "identity" : objInit.identity || '',
    }
}
function createMessageItem(ObjInit) {
    return new MessageItem(ObjInit);

}
const uploadChatFile = function(formData){
    return connectivityModule.http.post("Tutoring/upload", formData);
};
export default {
    sendChatMessage: (message) => {
        return dataTrack.send(JSON.stringify(message));
    },
    createMessageItem,
    uploadChatFile
}