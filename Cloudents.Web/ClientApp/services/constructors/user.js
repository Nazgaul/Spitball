export const User = {
    Default: function (objInit) {
        this.id = objInit.id;
        this.name = objInit.name;
        this.image = objInit.image || '';
    },
    Tutor: function (objInit) {
        this.bio = objInit.bio;
        this.currency = objInit.currency;
        this.documents = objInit.documents;
        this.hasCoupon = objInit.hasCoupon;
        this.lessons = objInit.lessons;
        this.subjects = objInit.subjects;
        this.price = objInit.price || 0;
        this.rate = objInit.rate || 0;
        this.reviewCount = objInit.reviewCount || 0;
        this.discountPrice = objInit.discountPrice;
        this.firstName = objInit.firstName || '';
        this.lastName = objInit.lastName || '';
    },
}