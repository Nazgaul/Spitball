<template>
    <v-flex class="right-sidebar">
        <!--<askQuestionBtn class="ask-question-button" v-if="isAsk"></askQuestionBtn>-->
        <!--open upload-->
        <!--<upload-files-btn class="upload-document-button" v-if="isNotes" @click="openUploaderDialog()"></upload-files-btn>-->
        <v-flex xs12 class="card-block">
            <marketing-box class="mb-3"></marketing-box>
            <!--<leaders-board ></leaders-board>-->
            <tutorList></tutorList>
        </v-flex>
        <v-flex v-if="!!suggestList[name]" xs12 class="card-block mt-3">
        </v-flex>
    </v-flex>
</template>


<script>
    import {suggestList} from "./../../consts"
    import help from "../../../../services/satelliteService";
    import {mapGetters, mapActions } from 'vuex';
    import askQuestionBtn from '../askQuestionBtn/askQuestionBtn.vue';
    import uploadFilesBtn from "../uploadFilesBtn/uploadFilesBtn.vue";
    import marketingBox from "../../../helpers/marketingBox/marketingBox.vue";
    import tutorList from "../../../helpers/tutorList/tutorList.vue";
    export default {
        components:{
            askQuestionBtn,
            uploadFilesBtn,
            marketingBox,
            tutorList
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

            }),

        },
        methods:{
            ...mapActions(["updateLoginDialogState", 'updateUserProfileData',
            ]),


        },

        created() {
        }
    }
</script>

<style src="./faq-block.less" lang="less"></style>