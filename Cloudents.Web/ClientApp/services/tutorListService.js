import { connectivityModule } from "./connectivity.module";

function TutorItem(objInit) {
    this.userId = objInit.userId;
    this.name = objInit.name;
    this.image = objInit.image;
    this.bio = objInit.bio;
    this.price = objInit.price || 50;
    this.score = objInit.score;
    this.rating = objInit.rate || null;
}


export default {
    getTutorList: () =>{
        return connectivityModule.http.get("tutor").then(({data})=> {
            let result = [];
            if(!!data && data.length > 0){
                data.forEach((tutorItem)=>{
                    result.push(new TutorItem(tutorItem));
                });
            }
            return result;
        })
    }

};