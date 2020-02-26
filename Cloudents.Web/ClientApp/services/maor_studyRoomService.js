import axios from 'axios'
import {StudyRoom} from './Dto/StudyRoom.js';
const studyRoomInstance = axios.create({
    baseURL:'/api/studyRoom'
})

export default {
   async getRoomInformation(roomId){ 
      let {data} = await studyRoomInstance.get(`${roomId}`)
      data.roomId = roomId;
      return new StudyRoom.RoomProps(data)
   },
   async createStudyRoom(userId){ 
      let params = {userId};

      return await studyRoomInstance.post('',params)
   },
// TODO: change in chat store
}