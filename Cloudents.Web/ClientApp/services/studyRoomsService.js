import { connectivityModule } from "./connectivity.module"
// TODO: move it to studyroom service and change it in chat store
// TODO: move to studyroom service
const createRoom = (userId)=>{
    let params = {
        userId
    };
    return connectivityModule.http.post("StudyRoom", params);
};

export default {
    createRoom
}