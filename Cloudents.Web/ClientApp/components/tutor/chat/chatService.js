import { dataTrack } from '../tutorService';

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

export default {
    sendChatMessage: (message) => {
        return dataTrack.send(JSON.stringify(message));
    },
    createMessageItem
}