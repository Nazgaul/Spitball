export const StudyRoom = {
   RoomProps:function(objInit, roomId) {
      this.allowReview = true;
      this.conversationId = objInit.conversationId ;
      this.needPayment = objInit.needPayment;
      this.onlineDocument = objInit.onlineDocument || null;
      this.studentId = objInit.studentId;
      this.studentImage = objInit.studentImage || null;
      this.studentName = objInit.studentName;
      this.tutorId = objInit.tutorId;
      this.tutorImage = objInit.tutorImage || null;
      this.tutorName = objInit.tutorName;
      this.roomId = roomId;
      this.tutorPrice = objInit.tutorPrice;
      this.jwt = objInit.jwt || null;
   }
}