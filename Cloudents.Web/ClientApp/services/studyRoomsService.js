import { connectivityModule } from "./connectivity.module"

const createRoom = (userId)=>{
    let params = {
        userId
    };
    return connectivityModule.http.post("StudyRoom", params);
};

export default {
    createRoom
}