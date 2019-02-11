<template>
    <form class="suspend-container">
        <h1>Suspend User</h1>
        <div class="suspend-input-container">
            <v-text-field solo type="text" class="user-id-input" placeholder="Insert user id..." v-model="userIds"/>
        </div>
        <div class="suspend-input-container">
            <v-text-field solo type="text" class="user-id-input" placeholder="reason" v-model="reason" required/>
        </div>
        <div class="suspend-checkbox-container">
            <input type="checkbox" id="removeQuestion" v-model="deleteUserQuestions"> 
            <label for="removeQuestion">Remove Question </label>
        </div>
        <div class="suspend-button-container">
            <v-btn  :loading="suspendLoading" color="red" @click.prevent="actionUser(false)" :class="{'lock': lock}">Suspend</v-btn>
            <v-btn  :loading="releaseLoading" color="green" @click.prevent="actionUser(true)" :class="{'lock': lock}">Release</v-btn>
        </div>
        <div v-if="showSuspendedDetails" class="suspended-user-container">
            <h3>Email: {{suspendedMail}}</h3>
        </div>
    </form>
</template>

<script>
import {suspendUser, releaseUser} from './suspendUserService'
export default {
    data(){
        return{
            serverIds: [],
            deleteUserQuestions:false,
            showSuspendedDetails: false,
            suspendedMail: null,
            lock: false,
            suspendLoading: false,
            releaseLoading: false,
            reason: null
        }
    },
    props: {
        userIds:{
            type: String,
            default: ''
        }
    },
    methods:{
        actionUser:function(unsuspendUser){
            if(!this.userIds){
                this.$toaster.error("Please Insert A user ID")
                return;
            }
            this.serverIds = [];
            this.userIds.split(',').forEach(id=>{
                let num = parseInt(id.trim());
                if(!!num){
                    return this.serverIds.push(num);
                }  
            });
            
            this.lock = true;
            if(!!unsuspendUser){
                this.releaseLoading = true;
                releaseUser(this.serverIds).then((email)=>{
                    this.$toaster.success(`user got released`);
                    this.releaseLoading = false;
                    this.reason = null;
                    this.userIds = null;
                }, (err)=>{
                    this.$toaster.error(`ERROR: failed to realse user`);
                    console.log(err)
                }).finally(()=>{
                    this.lock = false;
                    this.releaseLoading = false;
                })
            }else{
                if(!this.reason){
                    this.$toaster.error("Please Insert A Reason")
                    this.lock = false;
                    return;
                }
                this.suspendLoading = true;
                suspendUser(this.serverIds, this.deleteUserQuestions, this.reason).then((email)=>{
                    this.$toaster.success(`user got suspended, email is: ${email}`)
                    this.showSuspendedDetails = true;
                    this.suspendedMail = email;
                    this.suspendLoading = false;
                    this.reason = null;
                    this.userIds = null;
                }, (err)=>{
                    this.$toaster.error(`ERROR: failed to suspend user`);
                    console.log(err)
                }).finally(()=>{
                    this.lock = false;
                    this.suspendLoading = false;
                })
            }
            
        }
    }
}
</script>

<style lang="scss" scoped>
.suspend-container{
    .suspend-input-container{
        justify-content: center;
        align-items: center;
        display: flex;
        flex-direction: column;
        .user-id-input{
            border: none;
            outline: none;
            border-radius: 25px;
            /*height: 15px;*/
            margin-top: 5px;
            padding: 10px;
            width: 345px;
        }
    }
    .suspend-checkbox-container{
        margin-top: 15px;
    }
    .suspend-button-container{
            margin-top: 15px;
            button{
                /*cursor: pointer;*/
                /*border: none;*/
                /*outline: none;*/
                /*background-color: #f35a5a;*/
                /*border-radius: 25px;*/
                /*height: 25px;*/
                /*width: 80px;*/
                /*color: #810000;*/
                /*font-weight: 600;*/
                &.lock{
                    background-color: #d1d1d1;
                    color: #a19d9d;         
                    pointer-events: none;
                }
            }
        }
}
</style>
