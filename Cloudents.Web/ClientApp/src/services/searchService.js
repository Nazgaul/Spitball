function TutorItem(objInit) {
    this.userId = objInit.userId;
    this.name = objInit.name || '';
    this.image = objInit.image;
    this.courses = objInit.courses || [];
    this.rating =  objInit.rate ? Number(objInit.rate.toFixed(2)): null;
    this.reviews = objInit.reviewsCount || 0;
    this.template = 'tutor-result-card';
    this.bio = objInit.bio || '';
    this.classes = objInit.classes || 0;
    this.lessons = objInit.lessons || 0;
    this.subjects = objInit.subjects || [];
    this.isTutor = true;
}

function createTutorItem(objInit) {
    return new TutorItem(objInit);
}

export default {
    createTutorItem,
}