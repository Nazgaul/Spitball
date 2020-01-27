export const HomePage = {
   Stats:function(objInit){
      this.documents = objInit.documents;
      this.tutors = objInit.tutors;
      this.students = objInit.students;
      this.reviews = objInit.reviews;
   },
   Review:function(objInit){
      this.text = objInit.text;
      this.userName = objInit.userName;
      this.tutorImage = objInit.tutorImage;
      this.tutorName = objInit.tutorName;
      this.tutorId = objInit.tutorId;
      this.tutorReviews = objInit.tutorReviews;
      this.tutorCount = objInit.tutorCount;
   }
}