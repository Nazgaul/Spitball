import axios from 'axios'

const studyRoomInstance = axios.create({
    baseURL:'/api/studyRoom'
})


const StudyRoom = {
   RoomProps:function(objInit) {
      this.allowReview = true;
      this.conversationId = objInit.conversationId || '';
      this.needPayment = objInit.needPayment;
      this.onlineDocument = objInit.onlineDocument || '';
      this.studentId = objInit.studentId || null;
      this.studentImage = objInit.studentImage || null;
      this.studentName = objInit.studentName || null;
      this.tutorId = objInit.tutorId || null;
      this.tutorImage = objInit.tutorImage || null;
      this.tutorName = objInit.tutorName || null;
      // this.isTutor = objInit.tutorId;
      this.roomId = objInit.roomId || '';
   }
}

export default {
   async getRoomInformation(roomId){ 
      let {data} = await studyRoomInstance.get(`${roomId}`)
      data.roomId = roomId;
      return new StudyRoom.RoomProps(data)
   },
}