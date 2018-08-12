import IQuestionDetail from './IQuestionDetail'
import Vue from 'vue'
import { Component, Prop } from 'vue-property-decorator'
import { aproveQuestion } from '@/components/question/questionComponents/mark/markService'

export class QuestionDetail implements IQuestionDetail {
    questionId: any;
    questionText: string;
    answerText: string;
    url: string;
    answerId: string;

    constructor(questionId: any, questionText: string, answerText: string, url: string, answerId: string){
        this.questionId= questionId;
        this.questionText= questionText;
        this.answerText= answerText;
        this.url= url;
        this.answerId= answerId;
    }
}

@Component({})
export default class QuestionDetailComponent extends Vue{ 
    @Prop({
        type: QuestionDetail,
        default: function(){return new QuestionDetail("Question Id", "QuestionText","AnswerText","Url","Aprove Url")}
    }) 
    question!: QuestionDetail        
    
    get questionDetail(){
        return this.question
    }   

    visitUrl(url: string){
        window.open(url, "_blank");
    }

    aproveQuestion(question: QuestionDetail){
        const aproveToServer = {
            QuestionId: question.questionId,
            AnswerId: question.answerId
        }
        let confirmed = window.confirm("are you sure you want to Aprove this question?")
        if(confirmed){
            aproveQuestion(aproveToServer);
        }
        
    }
    
}
    
    
