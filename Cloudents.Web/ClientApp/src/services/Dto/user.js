function _createIsTutorState(str){
    return str && str.toLowerCase() === 'ok'
}

export const User = {
    Default: function (objInit) {
        this.id = objInit.id || objInit.userId;
        this.name = objInit.name;
        this.firstName = objInit.firstName;
        this.lastName = objInit.lastName;
        this.image = objInit.image || '';
    },
    TutorDefault: function(objInit){
        this.price = objInit.price || 0;
        // this.currency = objInit.currency;
        this.bio = objInit.bio || '';
        this.lessons = objInit.lessons || 0;
        this.discountPrice = objInit.discountPrice;
        this.subjects = objInit.subjects.toString().replace(/,/g, ", ");
        this.pendingSessionsPayments = objInit.pendingSessionsPayments || null;
        this.description = objInit.description || '';
    },
    Tutor: function (objInit) {
        return{
                price : objInit.price || 0,
                // currency: objInit.currency,
                bio: objInit.bio || '',
                lessons: objInit.lessons || 0,
                discountPrice: objInit.discountPrice,
                // subjects: objInit.subjects.toString().replace(/,/g, ", "),
                pendingSessionsPayments: objInit.pendingSessionsPayments || null,
                description: objInit.description || '',
                contentCount: objInit.contentCount,
                hasCoupon: objInit.hasCoupon,
                rate: objInit.rate || 0,
                reviewCount: objInit.reviewCount || 0,
                firstName: objInit.firstName || '',
                lastName: objInit.lastName || '',
                students: objInit.students || 0,
                subscriptionPrice: objInit.subscriptionPrice,
                isSubscriber : objInit.isSubscriber
        }
        
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
                classes: objInit.classes || 0,
                isTutor: true,
            }
        )
    },
    Account: function(objInit){
        return {
            id: objInit.id,
            email: objInit.email,
            lastName: objInit.lastName,
            firstName: objInit.firstName,
            name: `${objInit.firstName} ${objInit.lastName}`,
            image: objInit.image || '',
            balance: objInit.balance,
            currencySymbol: objInit.currencySymbol,
            subscription: objInit.tutorSubscription,
            needPayment: objInit.needPayment,
            isTutor: _createIsTutorState(objInit.isTutor),
            isSold: objInit.isSold,
            pendingSessionsPayments: objInit.pendingSessionsPayments,
            chatUnread: objInit.chatUnread,
        }        
    },
    Stats: function(objInit){
        this.revenue = objInit.revenue
        this.sales = objInit.sales
        this.views = objInit.views
        this.followers = objInit.followers
    }
}