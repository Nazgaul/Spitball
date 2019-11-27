import { connectivityModule } from "./connectivity.module"
import { Tutoring } from './constructors.js'


// REMOVE IT
// function StudyRoom(objInit){
//     this.name = objInit.name;
//     this.image = objInit.image;
//     this.online = objInit.online;
//     this.id = objInit.id;
//     this.dateTime = objInit.dateTime;
//     this.conversationId = objInit.conversationId;
//     this.userId = objInit.userId;
// }

function createStudyRoom(objInit){
    return new Tutoring.StudyRoom(objInit);
}

const getRooms = () => {
    return connectivityModule.http.get("StudyRoom").then(({data})=>{
        let studyRooms = [];
        if(data.length > 0){
            studyRooms = data.map(createStudyRoom);
        }
        return studyRooms;
    });
};

const createRoom = (userId)=>{
    let params = {
        userId
    };
    return connectivityModule.http.post("StudyRoom", params);
};

export default {
    getRooms,
    createRoom
}