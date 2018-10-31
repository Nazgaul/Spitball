<template>
    <v-card class="mb-5 sb-step-card">
        <div class="upload-row-1 referal-row-1">
            <v-icon class="five">sbf-spread-loud</v-icon>
            <h3 class="sb-title">Spread the Word</h3>
        </div>
        <div class="upload-row-2 referral-row">
            <referral-dialog :popUpType="''"></referral-dialog>
        </div>
        <div class="upload-row-3 referal-row-3">
            <h3 class="sb-subtitle mb-3">Help others while you are waiting</h3>
            <div class="referal-btns-wrap">
                <v-btn round class="referal-ask" @click="closeAndGoTo('ask')">
                    <span>Ask a question</span>
                    <v-icon right class="referal-edit-icon ml-3">sbf-edit-icon</v-icon>

                </v-btn>
                <v-btn round outline class="sb-back-flat-btn referal-answer" @click="closeAndGoTo('answer')">
                    <span>Answer a question</span>
                </v-btn>
            </div>
        </div>
    </v-card>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import referralDialog from "../../../../question/helpers/referralDialog/referral-dialog.vue";

    export default {
        name: "uploadStep_7",
        components: { referralDialog },
        data() {
            return {}
        },
        props: {
            callBackmethods: {
                type: Object,
                default: {},
                required: false
            }
        },
        computed: {
            ...mapGetters({
                getLegal: 'getLegal',
                getFileData: 'getFileData',
                getSchoolName: 'getSchoolName'
            }),
        },
        methods: {
            ...mapActions(['updateLegalAgreement']),

            closeAndGoTo(val) {
                this.callBackmethods.closeAndOpenAsk(val)
            },

        },

    }
</script>

<style >

</style>