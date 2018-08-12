import { connectivityModule } from '@/services/connectivity.module'
import {QuestionDetail} from '../mark/questionDetail/questionDetail'

export const getAllQUestions = (callback:Function) : any=> {
    connectivityModule.get("MarkQuestion").then(({data}:any)=> {
        let questions:QuestionDetail[] = [];
        if(!!data &&data.length > 0){
            data.forEach((question:any)=>{
                questions.push(new QuestionDetail(question.questionId, question.questionText,question.answerText,question.url, question.answerId))
            })
        }
        if(callback){
            callback(questions);
        }else{
            return questions;
        }
        
    }, (err:any)=>{
        if(callback){
            callback([]);
        }else{
            return [];
        }
    });
}

export const aproveQuestion = (data:any) : any=> {
    connectivityModule.post("MarkQuestion", data).then(({data}:any)=> {
        window.alert('success');
    }, (err:any)=>{
        window.alert('Faild');
    });
    
}

    
    
