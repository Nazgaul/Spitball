import { Component } from 'vue-property-decorator'
import Question from '@/components/question/question'
import QuestionDetailComponent from '@/components/question/questionComponents/mark/questionDetail/questionDetail'
import { getAllQUestions } from './markService'


@Component({
    components: {QuestionDetailComponent}
})
export default class QMark extends Question{
    msg: string = "Hello From Mark"
    questions: any = getAllQUestions();
    
}