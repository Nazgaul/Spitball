<template>
    <v-flex class="right-sidebar">
        <askQuestionBtn v-if="isAsk"></askQuestionBtn>
        <!--open upload-->
        <a class="upload-files" @click="openUploaderDialog()">Upload Documents</a>
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
        <v-flex v-if="!!suggestList[name]" xs12 class="card-block mt-3">
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
    import askQuestionBtn from '../askQuestionBtn/askQuestionBtn.vue';
    import uploadFiles from "../../helpers/uploadFiles/uploadFiles.vue"
    export default {
        components:{
            askQuestionBtn,
            uploadFiles
        },
        data() {
            return {
                faqList: null,
                suggestList,
                showDialogLogin: false
            }
        },
        props: {
            name: {},
            text: {},
            isAsk: Boolean,
            isNotes: Boolean
        },
        computed: {
            ...mapGetters({
                accountUser: 'accountUser',
                loginDialogState: 'loginDialogState',
                getSelectedClasses: 'getSelectedClasses',
                getDialogState:'getDialogState'

            }),
            isClassesSet(){
                return this.getSelectedClasses.length > 0
            },
            showUploadDialog() {
                return this.getDialogState
            },
        },
        methods:{
            ...mapActions(["updateLoginDialogState", 'updateUserProfileData', 'updateDialogState', 'changeSelectPopUpUniState']),

            openUploaderDialog() {
                if (this.accountUser == null) {
                    this.updateLoginDialogState(true);
                } else if (this.accountUser.universityExists && !this.isClassesSet) {
                    this.updateDialogState(true);
                } else {
                    this.changeSelectPopUpUniState(true)
                }

            },
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