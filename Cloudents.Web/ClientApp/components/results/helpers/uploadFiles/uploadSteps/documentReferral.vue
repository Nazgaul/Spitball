<template>
    <v-card class="mb-3 sb-step-card">
        <div class="upload-row-1 referal-row-1">
            <!--<v-icon class="five">sbf-spread-loud</v-icon>-->
            <i class="five">
                <spreadOutLoudIcon style="width: 50px"></spreadOutLoudIcon>
            </i>
            <h3 class="sb-title" v-language:inner>upload_files_step7_title</h3>
        </div>
        <div class="upload-row-2 referral-row">
            <referral-dialog :userReferralLink="referalLink" :referralType="'uploadReffer'" :isTransparent="true" :popUpType="''"></referral-dialog>
        </div>
        <!--<div class="upload-row-3 referal-row-3">-->
            <!--&lt;!&ndash;<h3 class="sb-subtitle mb-3" v-language:inner>upload_files_step7_subtitle</h3>&ndash;&gt;-->
            <!--<h3 class="sb-subtitle mb-3" v-language:inner>upload_files_proccesing</h3>-->

            <!--<div class="referal-btns-wrap">-->
                <!--<v-btn round class="referal-ask" @click="closeAndGoTo()">-->
                    <!--<span v-language:inner>upload_files_btn_askQuestion</span>-->
                    <!--<v-icon right class="referal-edit-icon ml-3">sbf-edit-icon</v-icon>-->

                <!--</v-btn>-->
                <!--<v-btn round outline class="sb-back-flat-btn referal-answer" @click="goToaskQuestion()">-->
                    <!--<span v-language:inner>upload_files_btn_answer</span>-->
                <!--</v-btn>-->
            <!--</div>-->
        <!--</div>-->
    </v-card>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import Base62 from "base62"
    import referralDialog from "../../../../question/helpers/referralDialog/referral-dialog.vue";
    import spreadOutLoudIcon from '../../../../../font-icon/spread-loud.svg';
    export default {

        name: "uploadStep_7",
        components: { referralDialog, spreadOutLoudIcon },
        data() {
            return {

            }
        },
        props: {
            docReferral: {
                type: String,
                default: '',
                required: false
            },
        },
        computed: {
            ...mapGetters({
                getFileData: 'getFileData',
                getSchoolName: 'getSchoolName',
            }),
            referalLink(){
              return  `${global.location.origin}`+ this.docReferral +"?referral=" + Base62.encode(this.accountUser().id) + "&promo=referral";
            },
        },
        methods: {
            ...mapGetters({
                accountUser: 'accountUser'

            }),
            ...mapActions(['askQuestion', 'updateDialogState']),
            closeAndGoTo() {
                this.askQuestion(false)
            },
            goToaskQuestion(){
                this.$router.push({path: '/ask'});
                this.updateDialogState(false)
            }

        }
    }
</script>

<style >

</style>