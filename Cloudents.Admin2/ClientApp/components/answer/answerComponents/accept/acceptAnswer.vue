<template>
<div class="">
<h1>Accept Answer</h1>
<div class="wrap">
    <div class="info">
        <!-- <h3>To Accept multiple questions, enter the question Id's seperated with a comma (See example).</h3> -->
        <h4 v-if="infoSuccess">{{infoSuccess}}</h4>
        <h4 v-else-if="infoError">{{infoError}}</h4>
    </div>
    <div class="input-wrap">
        <v-text-field height="36" solo class="id-input" type="text" v-model="answersIdString" placeholder="example: 1245"/>
        <v-btn color="#97ed82" @click="acceptByIds" >Accept</v-btn>
    </div>
</div>
</div>  

</template>
<script>

import { acceptAnswer } from './acceptAnswerService'

export default {
    data(){
        return {
            answersIds:[],
            answersIdString:'',
            infoSuccess: '',
            infoError: ''            
        }
    },
    methods:{
            acceptByIds(){
                if(this.answersIdString.length > 0){
                    this.answersIds = this.answersIdString.split(',');
                    let numberArr= [];
                    this.answersIds.forEach(id=>{
                        return numberArr.push(id.trim());
                    })
                    acceptAnswer(numberArr)
                    .then(resp=>{
                        this.$toaster.success(`Answer ${this.answersIdString} were Accepted`);
                        this.answersIdString= '';
                        this.answersIds = [];
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

<style lang="less" scoped>
    .wrap {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        .input-wrap {
            padding-top: 16px;
            display: flex;
            flex-direction: row;
            align-items: baseline;
            justify-content: center;
            .id-input {
                width: 345px;
                height: 36px;
                border-radius: 25px;
                padding-left: 10px;
            }
        }
    }

</style>