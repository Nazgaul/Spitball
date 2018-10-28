<template>
<div class="">
<h1>Accept Question</h1>
<div class="wrap">
    <div class="info">
        <!-- <h3>To Accept multiple questions, enter the question Id's seperated with a comma (See example).</h3> -->
        <h4 v-if="infoSuccess">{{infoSuccess}}</h4>
        <h4 v-else-if="infoError">{{infoError}}</h4>
    </div>
    <div class="input-wrap">
        <input class="id-input" type="text" v-model="questionsIdString" placeholder="example: 1245">
        <button @click="acceptByIds" class="btn-accept">Accept</button>
    </div>
</div>
</div>  

</template>
<script>

import { acceptQuestion } from './acceptQuestionService'

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
            acceptByIds(){
                if(this.questionsIdString.length > 0){
                    this.questionsIds = this.questionsIdString.split(',');
                    let numberArr= [];
                    this.questionsIds.forEach(id=>{
                        return numberArr.push(parseInt(id.trim()));
                    })
                    acceptQuestion(numberArr)
                    .then(resp=>{
                        this.$toaster.success(`Question ${this.questionsIdString} were Accepted`);
                            this.questionsIdString= '';
                            this.questionsIds = [];
                    },
                    (error)=>{
                             this.$toaster.error('Something went wrong');
                            console.log('component accept error', error)
                    }
                    )
                }
            },
        },
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

.btn-accept { 
    cursor: pointer;
    background-color: #55ec51;
    border-radius: 25px;
    border: none;
    outline: none;
    cursor:pointer;
    height: 25px;
        
} 

</style>