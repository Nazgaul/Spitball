export const User = {
    Default: function (objInit) {
        this.id = objInit.id;
        this.name = objInit.name;
        this.image = objInit.image || '';
    },
    TutorDefault: function(objInit){
        this.price = objInit.price || 0;
        this.currency = objInit.currency;
        this.bio = objInit.bio || '';
        this.lessons = objInit.lessons || 0;
        this.discountPrice = objInit.discountPrice;
        this.subjects = objInit.subjects || [];
    },
    Tutor: function (objInit) {
        return Object.assign(
            new User.TutorDefault(objInit),
            {
                documents: objInit.documents,
                hasCoupon: objInit.hasCoupon,
                rate: objInit.rate || 0,
                reviewCount: objInit.reviewCount || 0,
                firstName: objInit.firstName || '',
                lastName: objInit.lastName || '',
            }
        )

    },
    TutorItem: function(objInit){
        return Object.assign(
            new User.Default(objInit),
            new User.TutorDefault(objInit),
            {
                courses: objInit.courses || [],
                country: objInit.country,
                rating:  objInit.rate ? Number(objInit.rate.toFixed(2)): null,
                reviews: objInit.reviewsCount || 0,
                template: 'tutor-result-card',
                university: objInit.university || '',
                classes: objInit.classes || 0,
                isTutor: true,
            }
        )
    }
}