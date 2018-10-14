<template>
    <div class="add-question-container">
        <div class="legend-menu">
            <h2>Subject Id's</h2>
            <div v-for="subject in this.subjects" :key="subject.id">
                <span>{{subject.subject}}</span>
                <span> = <b>{{subject.id}}</b></span>
                
            </div>
        </div>

        <div class="added-questions-legend" v-if="addedQuestions > 0">
            <h2>{{addedQuestions}} Questions Added</h2>
        </div>
  
        <div>
            <h1>Add Multiple Questions</h1>

            <div class="text-area-container">
                <textarea v-model="subjectContent" :placeholder="areaPlaceHolder" cols="90" rows="50"></textarea>    
            </div>

            <div class="add-container">
                <button class="btn-add" @click="addQuestions">Add</button>
            </div>
        </div>

        <div class="error-questions-legend" v-if="questionError">
            <h2>{{questionErrorMessage}}</h2>
        </div>
    </div>
</template>

<script>
import { getSubjectList, addQuestion } from './addBulkQuestionsService'

export default {
    data(){
        return{
            subjectContent: "",
            areaPlaceHolder: `
            example:
            [
                {
                    "subjectId": 1,
                    "text": "mathematics related question",
                    "price": 100
                },
                {
                    "subjectId": 22,
                    "text": "sports related questionng",
                    "price": 50
                }
            ]`,
            subjects:'',
            addedQuestions: 0,
            questionError:false,
            questionErrorMessage: ""
        }
    },
    methods:{
        addQuestions: function(){
            this.addedQuestions = 0;
            this.questionError = false;
            try{
                let arrQuestions = JSON.parse(this.subjectContent.trim());
                if(arrQuestions.length > 0){
                arrQuestions.forEach((question)=>{
                    addQuestion(question.subjectId, question.text, question.price, this.addQuestionToList).then(()=>{   
                        this.addedQuestions++; 
                    }, (err)=>{
                        console.log(err);
                        alert("Error: Failed to Add question");
                    })
                })
                
            }
            }catch(err){
                console.log("format Error")
                this.questionError = true;
                this.questionErrorMessage = "Format is Wrong make sure you write the the code the same way as the example"
            }
        }
    },
    created(){
        getSubjectList().then((responseSubjects)=>{
            this.subjects = responseSubjects
        })
    }
}
</script>

<style lang="scss" scoped>
.add-question-container{
    flex-direction: row;
    justify-content: center;
    .legend-menu{
        padding-left: 8%;
        position: absolute;
        padding-top: 5%;
    }
    .added-questions-legend{
        padding-right: 8%;
        position: absolute;
        padding-top: 5%;
        right:0;
    }
    .error-questions-legend{
        color:red
    }
    .text-area-container{
        textarea{
            margin-top: 25px;
            border: none;
            background-color: rgb(180, 224, 165);
            color: rgb(124, 121, 121);
            outline: none;
            width: 545px;
            height: 570px;
            padding:10px;
        }
    }
    .add-container{
        margin-top:20px;
        .btn-add{
            background-color: #438a2b;
            border-radius: 15px;
            border: none;
            outline: none;
            cursor: pointer;
            height: 25px;
            width: 50px;
            color: #eaf0e9;
        }
    }
    
}
</style>