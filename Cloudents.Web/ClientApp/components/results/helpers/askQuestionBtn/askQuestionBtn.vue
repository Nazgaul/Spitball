<template>
    <v-flex xs12>
        <a class="ask-question" @click="goToAskQuestion()" v-language:inner>faqBlock_ask</a>
    </v-flex>
</template>
<script>
    import {mapGetters, mapActions } from 'vuex';
    export default {
        name: "askQuestionBtn",
         computed:{
            ...mapGetters({
                accountUser: 'accountUser',
            }),
        },
        methods:{
            ...mapActions(["updateLoginDialogState", 'updateUserProfileData', 'updateNewQuestionDialogState']),
            goToAskQuestion(){
                if(this.accountUser == null){
                    this.updateLoginDialogState(true);
                    //set user profile
                    this.updateUserProfileData('profileHWH')
                }else{
                    this.updateNewQuestionDialogState(true)
                }
            },
            // goToAskQuestion(){
            //     if(this.accountUser == null){
            //         this.updateLoginDialogState(true);
            //         //set user profile
            //         this.updateUserProfileData('profileHWH')
            //     }else{
            //         this.$router.push({name: 'newQuestion'});
            //     }
            // }
        },
        created(){
        }
    }
</script>

<style lang="less">
    @import "../../../../mixin.less";

    .ask-question{
        border-radius: 4px;
        //TODO ui changes
        //background-color: @color-dark-blue;
        background-color: @color-main-purple;
        color: @color-white;
        font-size: 20px;
        font-weight: bold;
        line-height: 40px;
        letter-spacing: -0.5px;
        width: 100%;
        display: block;
        text-align: center;
        margin-bottom: 17px;
        .transition (all .2s ease-in-out);
        &:hover {
            background-color: lighten(@color-main-purple, 10%);
        }
    }

</style>