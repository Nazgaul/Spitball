<template>
    <div class="user-tokens-container">
        <h1>Send Tokens To User</h1>
        <div class="user-inputs-container">
            <span><input class="user-input-text" type="text" v-model.number="userId" placeholder="Insert user id..."/></span>
            <span><input class="user-input-text" type="text" v-model.number="tokens" placeholder="Set amount of tokens to apply..."/></span>
        </div>
        <div class="select-type-container">
            <select class="select-type" v-model="tokenType">
                <option value="Earned">Earned</option>
                <option value="Awarded">Awarded</option>
            </select>
        </div>
        <div class="grant-token-container">
            <button class="grant-token-button" @click="sendTokens">Send</button>
        </div>
    </div>
    
</template>

<script>
import { grantTokens } from './tokenUserService'
export default {
    data(){
        return {
            userId: null,
            tokens: null,
            tokenType: "Earned"
        }
    },
    methods:{
        sendTokens: function(){
            if(!this.userId){
                alert("you must provide a UserId")
                return;
            }
            if(!this.tokens){
                alert("you must provide tokens")
                return;
            }
            grantTokens(this.userId, this.tokens, this.tokenType).then(()=>{
                alert(`user id ${this.userId} recived ${this.tokens} tokens`)
            },(err)=>{
                console.log(err);
                alert(`Error: couldn't send tokens`)
            })
            
        }
    }
}
</script>

<style lang="scss" scoped>
.user-tokens-container{
    .user-inputs-container{
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
    .grant-token-container{
        margin-top: 15px;
        .grant-token-button{
            cursor: pointer;
            border:none;
            outline: none;
            background-color: #78c967;
            border-radius: 25px;
            height:25px;
            width: 50px;
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
