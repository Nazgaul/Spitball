import { connectivityModule } from "./connectivity.module"


function StudyRoom(objInit){
    this.name = objInit.name;
    this.image = objInit.image;
    this.online = objInit.online;
    this.id = objInit.id;
    this.dateTime = objInit.dateTime;
    this.conversationId = objInit.conversationId;
    this.userId = objInit.userId;
}

function createStudyRoom(objInit){
    return new StudyRoom(objInit);
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
// const skipNeedPayment = (studyRoomId)=>{
//     return connectivityModule.http.post("StudyRoom/NoCard", studyRoomId)
// };

export default {
   // skipNeedPayment,
    getRooms,
    createRoom
}