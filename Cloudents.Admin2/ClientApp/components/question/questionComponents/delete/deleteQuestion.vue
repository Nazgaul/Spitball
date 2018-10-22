<template>
<div class="">
<h1>Delete Question</h1>
<div class="wrap">
    <div class="info">
        <h3>To Delete multiple questions, enter the question Id's seperated with a comma (See example).</h3>
        <h4 v-if="infoSuccess">{{infoSuccess}}</h4>
        <h4 v-else-if="infoError">{{infoError}}</h4>
    </div>
    <div class="input-wrap">
        <input class="id-input" type="text" v-model="questionsIdString" placeholder="example: 1245,6689,1123">
        <button @click="deleteByIds" class="btn-danger">Delete</button>
    </div>
</div>
</div>  

</template>
<script>

import {deleteQuestion} from './deleteQuestionService'

export default {
    data(){
        return {
            questionsIds:[],
            questionsIdString:'',
            infoSuccess: '',
            infoError: ''            
        }
    },
    methods:{
            deleteByIds(){
                if(this.questionsIdString.length > 0){
                    this.questionsIds = this.questionsIdString.split(',');
                    let numberArr= [];
                    this.questionsIds.forEach(id=>{
                        return numberArr.push(parseInt(id.trim()));
                    })
                    deleteQuestion(numberArr)
                    .then(resp=>{
                     
                        this.$toaster.success(`Questions were deleted: ${this.questionsIdString}`);
                            this.questionsIdString= '';
                            this.questionsIds = [];
                           

                    },
                    (error)=>{
                             this.$toaster.error('Something went wrong');
                            console.log('component delete error', error)
                    }
                    )
                }
            },
          
    },
    created(){
      
    }
} 
</script>

<style lang="scss" scoped>
.id-input{
      width: 400px;
    height: 25px;
    border-radius: 25px;
    background: #fff;
    border: none;
    outline: none;
    padding: 3px;
    padding-left: 10px;
}

.btn-danger { 
    cursor: pointer;
    background-color: #ec5151;
    border-radius: 25px;
    border: none;
    outline: none;
    cursor:pointer;
    height: 25px;
        
} 

</style>