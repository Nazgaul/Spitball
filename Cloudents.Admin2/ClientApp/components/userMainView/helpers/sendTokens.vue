<template>
    <div class="user-tokens-container">
        <div class="user-inputs-container">
            <v-text-field solo class="user-input-text" type="text" v-model.number="tokens" placeholder="Set amount of tokens to apply..."/>
        </div>
        <!--<div class="select-type-container">-->
            <!--<v-select attach=""-->
                      <!--class="select-type-input"-->
                      <!--solo-->
                      <!--v-model="tokenType"-->
                      <!--:items="types"-->
                      <!--:item-value="tokenType"-->
                      <!--label="Select type"-->
            <!--&gt;</v-select>-->
        <!--</div>-->
        <div class="grant-token-container">
            <v-btn round color="#78c967" @click="sendTokens">Send</v-btn>
        </div>
    </div>
</template>

<script>
    import { grantTokens } from '../../user/token/tokenUserService'
    export default {
        data(){
            return {
                tokens: null,
                tokenType: 'Earned',
                types: ['Earned'],

            }
        },
        props: {
            userId: {
                type: Number,
                default: 0
            },
        },
        methods:{
            sendTokens: function(){
                if(!this.tokens){
                    this.$toaster.error("you must provide tokens");
                    return;
                }
                grantTokens(this.userId, this.tokens, this.tokenType).then(()=>{
                    this.$toaster.success(`user id ${this.userId} recived ${this.tokens} tokens`)
                },(err)=>{
                    console.log(err);
                    this.$toaster.error(`Error: couldn't send tokens`)
                })

            }
        }
    }
</script>

<style scoped>

</style>