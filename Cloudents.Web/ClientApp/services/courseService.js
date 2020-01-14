import { connectivityModule } from "./connectivity.module";

const Course = {
   Default:function(objInit){
      this.name = objInit.name;
   }
}

function createSubject(objInit){
   if(!!objInit){
      return new Course.Default(objInit)
   }
}

function getSubject(params){
   if(!params) return

   return connectivityModule.http.get(`/Course/subject`,{params}).then((res)=>{
      return createSubject(res.data)
   });
}

export default {
   getSubject,
}