<template>
    <div class="user-tokens-container">
        <h1>Send Tokens To User</h1>
        <div class="user-inputs-container">
           <v-text-field   solo class="user-input-text" type="text" v-model.number="userId" placeholder="Insert user id..."/>
           <v-text-field solo class="user-input-text" type="text" v-model.number="tokens" placeholder="Set amount of tokens to apply..."/>
        </div>
        <div class="select-type-container">
            <select class="select-type" v-model="tokenType">
                <option value="Earned">Earned</option>
                <option value="Awarded">Awarded</option>
            </select>
        </div>
        <div class="grant-token-container">
            <v-btn round color="#78c967" @click="sendTokens">Send</v-btn>
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
            tokenType: 'Earned',
            types: ['Earned', 'Awarded'],
            select: null

        }
    },
    methods:{
        sendTokens: function(){
            if(!this.userId){
                this.$toaster.error("you must provide a UserId")
                return;
            }
            if(!this.tokens){
                this.$toaster.error("you must provide tokens")
                return;
            }
            grantTokens(this.userId, this.tokens, this.tokenType).then(()=>{
                this.$toaster.success(`user id ${this.userId} recived ${this.tokens} tokens`)
                this.userId= null;
                this.tokens= null;
            },(err)=>{
                console.log(err);
                this.$toaster.error(`Error: couldn't send tokens`)
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
        align-items: center;
        .user-input-text{
            border:none;
            outline: none;
            border-radius: 25px;
            /*height: 15px;*/
            margin-top: 5px;
            padding:10px;
            width: 345px;
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
