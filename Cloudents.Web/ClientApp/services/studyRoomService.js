import axios from 'axios'
import {StudyRoom} from './Dto/StudyRoom.js';
const studyRoomInstance = axios.create({
    baseURL:'/api/studyRoom'
})

export default {
   async getRoomInformation(roomId){ 
      let {data} = await studyRoomInstance.get(roomId).catch((err)=>{
         return Promise.reject(err)
      })
      return new StudyRoom.RoomProps(data,roomId);
   },
   async endTutoringSession(roomId){ 
      return await studyRoomInstance.post(`${roomId}/end`)
   },
   async uploadCanvasImage(formData){ 
      return await studyRoomInstance.post("upload",formData)
   },
   async enterRoom(roomId){ 
      let {data} = await studyRoomInstance.post(`${roomId}/enter`)
      return data.jwtToken;
   },
   async createRoom(name, userId){
      return await studyRoomInstance.post('',{
         name,
         userId
      });
   }
}