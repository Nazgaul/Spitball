import { Component } from 'vue-property-decorator'
import Question from '@/components/question/question'
import QuestionDetailComponent, { QuestionDetail } from '@/components/question/questionComponents/mark/questionDetail/questionDetail'
import { getAllQUestions } from './markService'
import { inherits } from 'util';


@Component({
    components: {QuestionDetailComponent}
})
export default class QMark extends Question{
    msg: string = "Hello From Mark"
    filter: string = ""
    
    questions: QuestionDetail[] = [];
        
    get filteredQuestions(){
        if(this.filter === ""){
            return this.questions;
        }
        return this.questions.filter((question:QuestionDetail) => {
            return question.questionId.toString().indexOf(this.filter) > -1  || question.questionText.indexOf(this.filter) > -1 || question.answerText.indexOf(this.filter) > -1;
        })
    }

    pushQuestionsData(data:any){
        this.questions = data;
    }

    init(){
        getAllQUestions(this.pushQuestionsData)   
    }

    constructor(){
        super();
        this.init();
    }

}