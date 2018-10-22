<template>
    <div class="suspend-container">
        <h1>Suspend User</h1>
        <div class="suspend-input-container">
            <input type="number" class="user-id-input" placeholder="Insert user id..." v-model.number="userId"/>
        </div>
        <div class="suspend-checkbox-container">
            <input type="checkbox" v-model="deleteUserQuestions"> Remove Question 
        </div>
        <div class="suspend-button-container">
            <button @click="banUser">Suspend</button>
        </div>

        <div v-if="showSuspendedDetails" class="suspended-user-container">
            <h3>Email: {{suspendedMail}}</h3>
        </div>
    </div>
</template>

<script>
import {suspendUser} from './suspendUserService'
export default {
    data(){
        return{
            userId: null,
            deleteUserQuestions:false,
            showSuspendedDetails: false,
            suspendedMail: null
        }
    },
    methods:{
        banUser:function(){
            if(!this.userId){
                this.$toaster.error("Please Insert A user ID")
                return;
            }
            suspendUser(this.userId, this.deleteUserQuestions).then((email)=>{
                this.$toaster.success(`userId ${this.userId} got suspended, email is: ${email}`)
                this.showSuspendedDetails = true;
                this.suspendedMail = email;
            }, (err)=>{
                this.$toaster.error(`ERROR: failed to suspend user`);
                console.log(err)
            })
        }
    }
}
</script>

<style lang="scss" scoped>
.suspend-container{
    .suspend-input-container{
        .user-id-input{
            border: none;
            outline: none;
            border-radius: 25px;
            height: 15px;
            margin-top: 5px;
            padding: 10px;
            width: 200px;
        }
    }
    .suspend-checkbox-container{
        margin-top: 15px;
    }
    .suspend-button-container{
            margin-top: 15px;
            button{
                cursor: pointer;
                border: none;
                outline: none;
                background-color: #f35a5a;
                border-radius: 25px;
                height: 25px;
                width: 80px;
                color: #810000;
                font-weight: 600;
            }
        }
}
</style>
