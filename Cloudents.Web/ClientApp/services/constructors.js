
// globalConstructor = 
const HomePage = {
    Stats: function(objInit){
        this.documents = objInit.documents;
        this.tutors = objInit.tutors;
        this.students = objInit.students;
        this.reviews = objInit.reviews;
    },
    Review: function(objInit){
        this.text = objInit.text;
        this.userName = objInit.userName;
        this.tutorImage = objInit.tutorImage;
        this.tutorName = objInit.tutorName;
        this.tutorId = objInit.tutorId;
        this.tutorReviews = objInit.tutorReviews;
    }
}
const User = {
    Default: function(objInit){
        this.image = objInit.image;
        this.name = objInit.name;
        this.userId = objInit.userId;
    }  
}
const Tutoring = {
    StudyRoom: function(objInit) {
        return Object.assign(
            new User.Default(objInit),
            {   
                online: objInit.online,
                id: objInit.id,
                dateTime: objInit.dateTime,
                conversationId: objInit.conversationId,
            }
        )
    }
}

export{
    HomePage,
    User,
    Tutoring
}

