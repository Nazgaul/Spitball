<template>
    <div class="user-tokens-container">
        <h1 align="center">Send Tokens To User</h1>
        <div class="user-inputs-container">
           <v-text-field   solo class="user-input-text" type="text" v-model.number="userId" placeholder="Insert user id..."/>
           <v-text-field solo class="user-input-text" type="text" v-model.number="tokens" placeholder="Set amount of tokens to apply..."/>
        </div>
        <div class="select-type-container">
            <v-select attach=""
                      class="select-type-input"
                      solo
                    v-model="tokenType"
                    :items="types"
                     :item-value="tokenType"
                    label="Select type"
            ></v-select>
            <div class="grant-token-container">
            <v-btn :loading="loading" color="#78c967" @click="sendTokens">Send</v-btn>
        </div>
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
            types: ['Earned'],
            loading: false

        }
    },
    methods:{
        sendTokens: function(){
            this.loading= true;
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
                this.loading= false;
            },(err)=>{
                console.log(err);
                this.$toaster.error(`Error: couldn't send tokens`);
                this.loading= false;
            })
            
        }
    }
}
</script>

<style lang="less" scoped>
.user-tokens-container{
    .user-inputs-container, .select-type-container{
        display:flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        .user-input-text, .select-type-input{
            border:none;
            outline: none;
            border-radius: 25px;
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

}

</style>
