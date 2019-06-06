
import {connectivityModule} from '../../services/connectivity.module'

function TutorItem(objInit) {
    this.userId = objInit.userId || 12;
    this.name = objInit.name || '';
    this.image = objInit.image;
    this.courses = objInit.courses || '';
    this.price = objInit.price || 50;
    this.score = objInit.score || null;
    this.rating =  objInit.rate ? Number(objInit.rate.toFixed(2)): null;
    this.reviews = objInit.reviewsCount || 0;
    this.template = 'tutor';
    this.bio = objInit.bio || '';

}

const createTutorItem = function(objInit){
    return new TutorItem(objInit);
}

const getTutorList = (params) => {
    return connectivityModule.http.get("tutor/search", { params }).then(({data})=>{
        return data.result.map(createTutorItem);
    })
}

export default{
    getTutorList
}
