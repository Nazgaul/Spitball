import { Component } from 'vue-property-decorator'
import Question from '@/components/question/question';

@Component({})
export default class QAdd extends Question{
    msg: string = "Hello From add"
}