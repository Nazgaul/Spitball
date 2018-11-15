<template>
    <div class="delete-user-container">
        <h1>Delete User</h1>
        <div class="user-inputs-container">
            <input class="user-input-text" placeholder="User Id..." type="text" v-model="userId"/>
        </div>
        <div class="delete-button-container">
            <button class="delete-button" :class="{'disabled': lock}" @click="deleteUser()">Delete</button>
        </div>
    </div>
</template>

<script>
import { deleteUser } from './deleteUserService.js';
export default {
    data(){
        return{
            userId: "",
            lock: false
        }
    },
    methods:{
        deleteUser(){
            if(this.userId === "") return ;
            this.lock = true;
            deleteUser(this.userId).then(()=>{
                this.$toaster.success(`User Deleted`);
                this.userId = "";
            },(err)=>{
                console.log(err);
                this.$toaster.error(`Error: couldn't delete user`)
            }).finally(()=>{
                this.lock = false;
            })
        }
    }
}
</script>

<style lang="scss" scoped>
    .delete-user-container{
    display: flex;
    flex-direction: column;
    .user-inputs-container{
            margin:0 auto;
            display:flex;
            flex-direction: column;
            justify-content: center;
            .user-input-text{
                border:none;
                outline: none;
                border-radius: 25px;
                height: 15px;
                margin-top: 5px;
                padding:10px;
                width: 200px;
            }
        }
    .delete-button-container{
        margin-top: 15px;
        .delete-button{
            cursor: pointer;
            border:none;
            outline: none;
            background-color: #c96767;
            border-radius: 25px;
            height:25px;
            width: 75px;
            &.disabled{
                background-color: #aeabab;
                pointer-events: none;
                color: #837d7d;
            }
        }
    }
}
</style>
