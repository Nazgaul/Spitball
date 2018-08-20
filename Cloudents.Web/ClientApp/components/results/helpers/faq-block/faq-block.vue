<template>
    <v-flex class="right-sidebar">
        <v-flex xs12 v-if="isAsk">
            <a class="ask-question" @click="goToAskQuestion()">Ask Your Question</a>
        </v-flex>
        <v-flex xs12 class="card-block">
            <div class="header">Spitball FAQ</div>
            <div class="content">
                <ul class="list">
                    <li v-for="(item,id) in faqList" v-if="id<5">
                        <router-link :to="{name:'faq',query:{id:id}}" target='_blank'>{{item.question}}</router-link>
                    </li>
                </ul>
            </div>
            <div class="footer">
                <router-link tag="button" to="/faq">More</router-link>
            </div>
        </v-flex>

        <v-flex xs12 class="card-block mt-3">
            <div class="header">Spitball</div>
            <div class="content">
                <p>{{suggestList[name]}}</p>
            </div>
            <div class="footer">
                <router-link tag="button" :to="{path:'/'+name,query:{q:text}}">Show me</router-link>
            </div>
        </v-flex>

        <sb-dialog :showDialog="loginDialogState" :popUpType="'loginPop'"  :content-class="'login-popup'">
            <login-to-answer></login-to-answer>
        </sb-dialog>
    </v-flex>
</template>


<script>
    import {suggestList} from "./../../consts"
    import help from "../../../../services/satelliteService";
    import sbDialog from '../../../wrappers/sb-dialog/sb-dialog'
    import loginToAnswer from '../../../question/helpers/loginToAnswer/login-answer'
    import {mapGetters, mapActions } from 'vuex';
    export default {
        components:{
            sbDialog,
            loginToAnswer

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
            ...mapActions(["updateLoginDialogState"]),
            goToAskQuestion(){
                if(this.accountUser == null){
                    this.updateLoginDialogState(true);
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