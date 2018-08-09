import { Component } from 'vue-property-decorator'
import Question from '@/components/question/question'
import QuestionDetailComponent, { QuestionDetail } from '@/components/question/questionComponents/mark/questionDetail/questionDetail'
import { getAllQUestions } from './markService'


@Component({
    components: {QuestionDetailComponent}
})
export default class QMark extends Question{
    msg: string = "Hello From Mark"
    questions: any = getAllQUestions();   
    createQuestionDetail(quantity: number = 5) : QuestionDetail[]{
        let list: QuestionDetail[] = []
        list.push(new QuestionDetail("Question Id", "QuestionText","AnswerText","Link","Aprove Url"));
        for(let i = 0; i < quantity; i++){
            list.push(new QuestionDetail(i, `text${i}`,`text${i}`,`text${i}`,`text${i}`))
        }
        return list;
    } 
    //questions: QuestionDetail[] = this.createQuestionDetail(5);


    // filterQuestions(text: string){
    //     this.questions = this.questions.filter((question)=>{
    //     })
    // }
}