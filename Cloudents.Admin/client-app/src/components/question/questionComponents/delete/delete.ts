import { Component } from 'vue-property-decorator'
import Question from '@/components/question/question';

@Component({})
export default class QDelete extends Question{
    title: string = "Delete Question"
    questionsToDelete: string = "";

    deleteQ(){
        //todo delete api
        if(this.questionsToDelete !== ""){
            console.log(this.questionsToDelete);
            alert(`Question id's: ${this.questionsToDelete} removed.`);
            this.questionsToDelete = "";
            
        }
    }
}