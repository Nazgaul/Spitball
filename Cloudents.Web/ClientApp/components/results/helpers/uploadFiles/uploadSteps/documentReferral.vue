<template>
    <v-card elevation="0" class="mb-3 sb-step-card">
        <div class="upload-row-1 referal-row-1">
            <!--<v-icon class="five">sbf-spread-loud</v-icon>-->
            <i class="five">
                <spreadOutLoudIcon style="width: 50px"></spreadOutLoudIcon>
            </i>
            <h3 class="sb-title" v-language:inner>upload_multiple_files_referral_title</h3>
        </div>
        <div class="upload-row-2 referral-row">
            <referral-dialog :userReferralLink="referalLink" :referralType="'uploadReffer'" :isTransparent="true" :popUpType="''"></referral-dialog>
        </div>
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