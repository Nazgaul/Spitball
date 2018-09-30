<template>
    <v-flex class="right-sidebar">
        <askQuestionBtn v-if="isAsk"></askQuestionBtn>
        <v-flex xs12 class="card-block">
            <div class="header" v-language:inner>faqBlock_faq</div>
            <div class="content">
                <ul class="list">
                    <li v-for="(item,id) in faqList" v-if="id<5" :key="id">
                        <router-link :to="{name:'faq',query:{id:id}}" target='_blank'>{{item.question}}</router-link>
                    </li>
                </ul>
            </div>
            <div class="footer">
                <router-link class="footer-btn" tag="button" to="/faq" v-language:inner>faqBlock_more</router-link>
            </div>
        </v-flex>
        <v-flex xs12 class="card-block mt-3">
            <div class="header" v-language:inner>faqBlock_spitball</div>
            <div class="content">
                <p>{{suggestList[name]}}</p>
            </div>
            <div class="footer">
                <router-link class="footer-btn" tag="button" :to="{path:'/'+name,query:{term:text}}" v-language:inner>faqBlock_show_me</router-link>
            </div>
        </v-flex>
    </v-flex>
</template>


<script>
    import {suggestList} from "./../../consts"
    import help from "../../../../services/satelliteService";
    import {mapGetters, mapActions } from 'vuex';
    import askQuestionBtn from '../askQuestionBtn/askQuestionBtn.vue'
    export default {
        components:{
            askQuestionBtn

        },
        data() {
            return {
                faqList: null,
                suggestList,
                showDialogLogin: false
            }
        },
        props: {name: {}, text: {}, isAsk: Boolean},
        computed: {
            ...mapGetters({
                accountUser: 'accountUser',
                loginDialogState: 'loginDialogState'

            }),
        },
        methods:{
            ...mapActions(["updateLoginDialogState", 'updateUserProfileData']),


            goToAskQuestion(){
                if(this.accountUser == null){
                    this.updateLoginDialogState(true);
                    //set user profile
                    this.updateUserProfileData('profileHWH')
                }else{
                    this.$router.push({name: 'newQuestion'});
                }
            }

        },

        created() {
            var self = this;
            help.getFaq().then(function (response) {
                self.faqList = response.data;
            })
        }
    }
</script>

<style src="./faq-block.less" lang="less"></style>