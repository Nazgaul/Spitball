<template>
    <div class="add-question-container">
        <!-- <div class="legend-menu">
            <h2>Subject Id's</h2>
            <div v-for="subject in this.subjects" :key="subject.id">
                <span>{{subject.subject}}</span>
                <span> = <b>{{subject.id}}</b></span>
                
            </div>
        </div> -->
        <div>
            <h1>Add Multiple Questions</h1>
            <div class="example-container">
                <span>Example</span>
                <img style="width: 100%; min-width: 600px" src="../../../../assets/img/exampleLast.png" alt="">
            </div>
            <!-- Deprecated -->
            <!-- <div class="text-area-container">
                <textarea v-model="subjectContent" :placeholder="areaPlaceHolder" cols="90" rows="50"></textarea>    
            </div> -->

            <div class="add-Upload-container">
                <input class="btn-upload" type="file" id="csvFileUpload" accept=".csv">
            </div>

            <div class="add-container">
                <v-btn :loading="loading"  :class="{'disabled': addDisabled}" @click="addQuestions">Add</v-btn>
            </div>

            <div class="added-questions-legend" v-if="addedQuestions > 0">
                <h2>{{addedQuestions}} Questions Added</h2>
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
            addDisabled: true,
            subjects:'',
            addedQuestions: 0,
            questionError:false,
            questionErrorMessage: "",
            questionsToUpload: null,
            loading: false
        }
    },
    methods:{
        uploadFile:function(event){
            let file = event.target.files[0];
            let reader = new FileReader();
            reader.readAsText(file)
            reader.onload = (event)=> {
            // Return array of string values, or NULL if CSV string not well formed.
            function CSVtoArray(text) {
            let p = '', row = [''], ret = [row], i = 0, r = 0, s = !0, l;
                for (l of text) {
                    if ('"' === l) {
                        if (s && l === p) row[i] += l;
                        s = !s;
                    } else if (',' === l && s) l = row[++i] = '';
                    else if ('\n' === l && s) {
                        if ('\r' === p) row[i] = row[i].slice(0, -1);
                        row = ret[++r] = [l = '']; i = 0;
                    } else row[i] += l;
                    p = l;
                }
                return ret;
            };

                console.log(event.target.result);
                let arrData	= CSVtoArray(event.target.result);
                let csvToJson = arrData.map(item=>{
                    return {
                        "course": item[0],
                        "text": item[1],
                        "country": item[2],
                        "uni": item[3],

                    }
                })

                console.log(csvToJson);
                this.addDisabled = false;
                this.questionsToUpload = csvToJson;
		};
            
        },
        addQuestions: function(){
            this.loading= true;
            this.addedQuestions = 0;
            this.questionError = false;
            let questions = this.questionsToUpload;
            let self = this;
            try{
                if(questions && questions.length > 0){
                questions.forEach((question)=>{
                    addQuestion(question.course, question.text, question.country, question.uni).then(()=>{
                        this.addedQuestions++; 
                        if(this.addedQuestions === this.questionsToUpload.length){
                            this.questionsToUpload = null;
                            document.getElementById('csvFileUpload').value = '';
                            this.addDisabled = true;
                            this.loading= false;

                        }

                    }, (err)=>{
                        console.log(err);
                        let badFormatQuestion = err.config.data;
                        self.questionError = true;
                        self.questionErrorMessage += `Wrong Question Data: ${badFormatQuestion}\n\n`
                        this.loading= false;

                    })
                })
                
            }
            }catch(err){
                console.log("format Error")
                this.questionError = true;
                this.questionErrorMessage = "Format is Wrong make sure you write the the code the same way as the example"
            }
            // try{
            //     let arrQuestions = JSON.parse(this.subjectContent.trim());
            //     if(arrQuestions.length > 0){
            //     arrQuestions.forEach((question)=>{
            //         addQuestion(question.subjectId, question.text, question.price, this.addQuestionToList).then(()=>{   
            //             this.addedQuestions++; 
            //         }, (err)=>{
            //             console.log(err);
            //             alert("Error: Failed to Add question");
            //         })
            //     })
                
            // }
            // }catch(err){
            //     console.log("format Error")
            //     this.questionError = true;
            //     this.questionErrorMessage = "Format is Wrong make sure you write the the code the same way as the example"
            // }
        }
    },
    created(){
        getSubjectList().then((responseSubjects)=>{
            this.subjects = responseSubjects
        })
        
    },
    mounted(){
        document.getElementById('csvFileUpload').addEventListener('change', this.uploadFile, false)
    }
}
</script>

<style lang="less" scoped>
.add-question-container{
    display: flex;
    flex-direction: row;
    justify-content: space-evenly;
    width: 100%;
    .legend-menu{
        padding-top: 5%;
    }
    .added-questions-legend{
        color:#4aa634;;
    }
    .error-questions-legend{
        white-space: pre-line;
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
    .add-Upload-container{
        .btn-upload{
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
        .disabled{
            background-color:gray;
            pointer-events: none;
        }
    }
    .example-container{
        display: flex;
        flex-direction: column;
        width: 600px;
        vertical-align: middle;
        margin: 70px auto;
    }
    
}
</style>