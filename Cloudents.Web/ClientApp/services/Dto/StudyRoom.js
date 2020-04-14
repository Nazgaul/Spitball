export const StudyRoom = {
   RoomProps:function(objInit, roomId) {
      this.conversationId = objInit.conversationId ;
      this.name = objInit.name;
      this.needPayment = objInit.needPayment;
      this.onlineDocument = objInit.onlineDocument || null;
      this.tutorId = objInit.tutorId;
      this.tutorImage = objInit.tutorImage || null;
      this.tutorName = objInit.tutorName;
      this.tutorPrice = objInit.tutorPrice;
      this.type = objInit.type;
      this.roomId = roomId;
      this.jwt = objInit.jwt || null;
      this.broadcastTime = objInit.broadcastTime || null;
      // TODO: check if we still need those:
      this.allowReview = true;
      this.studentId = objInit.studentId;
      this.studentImage = objInit.studentImage || null;
      this.studentName = objInit.studentName;
   }
}