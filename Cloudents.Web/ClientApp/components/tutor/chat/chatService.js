import { dataTrack } from '../tutorService';

function MessageItem(objInit) {
    this.type = 'tutoringChatMessage';
    this.data = {
        "text":  objInit.text || '',
        "user" : objInit.user || ''
    }
}
function createMessageItem(ObjInit) {
    return new MessageItem(ObjInit);

}

export default {
    sendChatMessage: (objInit) => {
        let message = createMessageItem(objInit);
        return dataTrack.send(JSON.stringify(message));
    },
    getMessage: () =>{
        dataTrack.on('tutoringChatMessage', data => {
          console.log('got data', data)
        });
    },
    createMessageItem
}