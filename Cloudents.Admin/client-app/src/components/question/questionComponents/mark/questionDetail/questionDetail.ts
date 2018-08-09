import IQuestionDetail from './IQuestionDetail'
import Vue from 'vue'
import { Component, Prop } from 'vue-property-decorator'

export class QuestionDetail implements IQuestionDetail {
    questionId: any;
    questionText: string;
    answerText: string;
    link: string;
    aproveUrl: string;

    constructor(questionId: any, questionText: string, answerText: string, link: string, aproveUrl: string){
        this.questionId= questionId;
        this.questionText= questionText;
        this.answerText= answerText;
        this.link= link;
        this.aproveUrl= aproveUrl;
    }
}

@Component({})
export default class QuestionDetailComponent extends Vue{ 
    @Prop({
        type: QuestionDetail,
        default: function(){return new QuestionDetail("Question Id", "QuestionText","AnswerText","Link","Aprove Url")}
    }) 
    question!: QuestionDetail        
    
    questionDetail:QuestionDetail =  this.question;
    
}
    
    
