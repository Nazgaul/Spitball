<template>
    <div class="add-question-container">
        <h1>Add A question</h1>

        First <b>Select</b> A subject: <select class="select-subject" name="subject"  @change="optionChanegd" v-model="selectedSubject">
            <option value="" disabled="true">Select...</option>
            <option v-for="subject in subjects" :value="subject.id" :key="subject.id">{{subject.subject}}</option>
        </select>
        <div class="text-area-container" v-if="showTextArea">
            <textarea v-model="subjectContent" placeholder="Content of text..." cols="30" rows="10"></textarea>    
        </div>

        <div class="price-container" v-if="showTextArea">
            <button  v-if="!showPriceSetter" class="btn-price" @click="setPrice">Set A Price</button>
            <div v-if="showPriceSetter">
                <input class="question-price" type="number" v-model="questionPrice" min="1"/>
            </div>
        </div>

        <div class="add-container" v-if="showPriceSetter">
            <button class="btn-add" @click="addQ">Add</button>
        </div>
    </div>
</template>

<script>
import { getSubjectList, addQuestion } from './addQuestionService'

export default {
    data(){
        return{
            subjects: [],
            selectedSubject: '',
            subjectContent: '',
            showTextArea:false,
            questionPrice: 1,
            showPriceSetter:false,
        }
    },
    methods:{
        optionChanegd: function(){
            this.showTextArea = true;
            console.log(this.selectedSubject)
        },
        setPrice:function(){
            this.showPriceSetter = true;
        },
        addQ: function(){
            if(this.questionPrice < 1){
                alert("Error: Price must be above 1");
                return;
            }
            if(this.subjectContent === ''){
                alert("Error: No Content");
                return;
            }
            addQuestion(this.selectedSubject, this.subjectContent, this.questionPrice).then(()=>{
                alert("Success on Adding Question");
            }, (err)=>{
                console.log(err);
                alert("Error: Failed to Add question");
            })
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
    .select-subject{
            border: none;
            background-color: #b4e0a5;
            height: 25px;
            color: #5d5d5d;
            outline: none;
        }
    .text-area-container{
        textarea{
            margin-top: 25px;
            border: none;
            background-color: rgb(180, 224, 165);
            color: rgb(124, 121, 121);
            outline: none;
            width: 500px;
            height: 150px;
            padding:10px;
        }
    }
    .price-container{
        margin-top:25px;
        .btn-price{
            cursor: pointer;
            background-color: #b6b6b6;
            border-radius: 25px;
            border: none;
            outline: none;
            cursor:pointer;
            height: 25px;
            color: #5d5d5d;
        }
        .question-price{
            margin-top:10px;
            padding:0 5px 0 5px;
            border: none;
            background-color: #b4e0a5;
            height: 25px;
            color: #5d5d5d;
            outline: none;
            border-radius: 25px;
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