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
        <div class="select-type-container" v-if="showTextArea">
            <select class="select-type" v-model="country">
                <option value="Us">US</option>
                <option value="Il">IL</option>
            </select>
        </div>
        <div class="upload-container" v-if="showTextArea">
            <file-upload
                ref="upload"
                v-model="files"
                post-action="/api/AdminQuestion/upload"
                @input-file="inputFile"
                @input-filter="inputFilter"
                :multiple="true"
                :maximum="4"
                :extensions="['jpeg', 'jpe', 'jpg', 'gif', 'png', 'webp']"
            >
                <button class="btn-upload">Upload File</button>
            </file-upload>
            <ul>
                <li v-for="(file, index) in files" :key="index">
                    {{file.name}} <span v-if="file.error">- Error: {{file.error}}</span> <span v-if="file.success">Success: {{file.success}}</span> 
                </li>
            </ul>
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
            files: [],
            filesNames: [],
            country: "Us"
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
                this.$toaster.error("Error: Price must be above 1");
                return;
            }
            if(this.subjectContent === ''){
                this.$toaster.error("Error: No Content");
                return;
            }
            let uploads = [];
            if(this.files.length > 0){
                this.files.forEach(file=>{
                    if(!!file.response.file){
                        uploads.push(file.response.file)
                    }
                })
            } 
            addQuestion(this.selectedSubject, this.subjectContent, this.questionPrice, this.country, uploads).then(()=>{
                this.$toaster.success("Success on Adding Question");
            }, (err)=>{
                console.log(err);
                this.$toaster.error("Error: Failed to Add question");
            })
        },
        inputFile(newFile, oldFile) {
            // Automatic upload
            if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
                if (!this.$refs.upload.active) {
                this.$refs.upload.active = true
                }
            }
            // Upload error
            if (newFile.error !== oldFile.error) {
                console.log('error', newFile.error, newFile)
            }

            // Uploaded successfully
            if (newFile.success !== oldFile.success) {
                console.log('success', newFile.success, newFile)
            }
        },
        inputFilter: function (newFile, oldFile, prevent) {
            if (newFile && !oldFile) {
                // Filter non-image file
                if (!/\.(jpeg|jpe|jpg|gif|png|webp)$/i.test(newFile.name)) {
                return prevent()
                }
            }

            // Create a blob field
            newFile.blob = ''
            let URL = window.URL || window.webkitURL
            if (URL && URL.createObjectURL) {
                newFile.blob = URL.createObjectURL(newFile.file)
            }
        },
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
            border-radius: 25px;
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
    .upload-container{
        margin-top:10px;
        .btn-upload{
            background-color: #438a2b;
            border-radius: 15px;
            border: none;
            outline: none;
            cursor: pointer;
            height: 40px;
            width: 85px;
            color: #eaf0e9;
        }
    }
    .select-type-container{
        .select-type{
            border: none;
            border-radius: 25px;
            height: 25px;
            margin-top: 10px;
            width: 90px;
            padding: 5px;
            outline: none;
        }
    }    
}
</style>