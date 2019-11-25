<template>
    <v-flex class="right-sidebar">
        <v-flex xs12 class="card-block">
            <marketing-box/>
        </v-flex>
        <v-flex v-if="!!suggestList[name]" xs12 class="card-block mt-3">
        </v-flex>
    </v-flex>
</template>


<script>
    import {suggestList} from "./../../consts"
    import help from "../../../../services/satelliteService";
    import {mapGetters, mapActions } from 'vuex';
    import uploadFilesBtn from "../uploadFilesBtn/uploadFilesBtn.vue";
    import marketingBox from "../../../helpers/marketingBox/marketingBox.vue";
    import tutorList from "../../../helpers/tutorList/tutorList.vue";
    export default {
        components:{
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
            ...mapActions(["updateLoginDialogState"]),


        },

        created() {
        }
    }
</script>

<style src="./faq-block.less" lang="less"></style>