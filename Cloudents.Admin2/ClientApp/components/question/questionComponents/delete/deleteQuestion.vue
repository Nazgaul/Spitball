<template>
<div class="">
<h1>Delete question</h1>
<div class="wrap">
    <div class="info">
        <h3>Please add question id to delete, in order to delete multiple questions, please use comma separated values, as 10,20,30.</h3>
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
                     
                        alert(`Questions were deleted: ${this.questionsIdString}`);
                            this.questionsIdString= '';
                            this.questionsIds = [];
                           

                    },
                    (error)=>{
                             alert('Something went wrong');
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
  width:400px;
  height: 30px;
  border-radius: 5px;
  background: #fff;
  border: 1px solid #ccc;
  outline:none;
  padding: 3px;
}
.id-input:focus{
  border:1px solid #56b4ef;
  box-shadow: 0px 0px 3px 1px #c8def0;
}
.btn-danger { 
  padding: 3px;
  height: 35px;
  width: 75px;
  color: #ffffff; 
  background-color: red; 
  border-color: #ffffff; 
} 
 
.btn-danger:hover, 
.btn-danger:focus, 
.btn-danger:active, 
.btn-danger.active, 
.open .dropdown-toggle.btn-danger { 
  color: #ffffff; 
  background-color: #49247A; 
  border-color: #130269; 
} 
 
.btn-danger:active, 
.btn-danger.active, 
.open .dropdown-toggle.btn-danger { 
  background-image: none; 
} 
 
.btn-danger.disabled, 
.btn-danger[disabled], 
fieldset[disabled] .btn-danger, 
.btn-danger.disabled:hover, 
.btn-danger[disabled]:hover, 
fieldset[disabled] .btn-danger:hover, 
.btn-danger.disabled:focus, 
.btn-danger[disabled]:focus, 
fieldset[disabled] .btn-danger:focus, 
.btn-danger.disabled:active, 
.btn-danger[disabled]:active, 
fieldset[disabled] .btn-danger:active, 
.btn-danger.disabled.active, 
.btn-danger[disabled].active, 
fieldset[disabled] .btn-danger.active { 
  background-color: #96444B; 
  border-color: #130269; 
} 
 
.btn-danger .badge { 
  color: #96444B; 
  background-color: #ffffff; 
}
</style>