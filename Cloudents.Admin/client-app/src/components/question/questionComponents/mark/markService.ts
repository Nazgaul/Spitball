import { connectivityModule } from '@/services/connectivity.module'
import QuestionDetail from '../mark/questionDetail/questionDetail'

export const getAllQUestions = () : any=> {
    connectivityModule.get("MarkQuestion").then((data:any)=> {
        let questions:QuestionDetail[] = [];
        if(!!data &&data.length > 0){
            data.foreach((question:any)=>{
                questions.push(new QuestionDetail({...question}))
            })
        }
        return questions;
    }, (err:any)=>{
        return [];
    });
    
}

    
    
