<template>
    <div class="user-consent-dialog-wrap pb-3">
        <v-layout class="pt-3">
            <v-flex xs12 class="text-xs-right px-3">
                <v-icon class="caption cursor-pointer" @click="closeDialog()">sbf-close</v-icon>
            </v-flex>
        </v-layout>
        <v-layout column align-center>
            <v-flex xs12 class="pt-12">
                <span class="subheading font-weight-bold" v-language:inner="'tutor_user_consent_dialog_title'"></span>
            </v-flex>
            <v-flex xs12 style="text-align: center;" class="pt-2">
                <span v-language:inner="'tutor_user_consent_dialog_message'"></span>
            </v-flex>
            <v-flex xs12 class="pt-4">
                <v-btn class="accept-consent-btn elevation-0 align-center justify-center" @click="accept()">
                    <span class="text-capitalize" v-language:inner="'tutor_user_consent_dialog_accept'"></span>
                </v-btn>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    import { mapActions } from 'vuex';
    import studyRoomRecordingService from '../../studyRoomRecordingService';

    export default {
        methods: {
            ...mapActions(['updateDialogUserConsent']),
            accept() {
                studyRoomRecordingService.activateRecord();
                this.closeDialog();
            },
            closeDialog() {
                this.updateDialogUserConsent(false);
            }
        },
    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .pt-12 {
        padding-top: 12px;
    }

    .user-consent-dialog-wrap {
        @BtnBackground: #ffc739;
        background: @color-white;
        border-radius: 4px;
        box-shadow: 0 3px 6px 0 rgba(0, 0, 0, 0.16);
        width: 356px;
        padding: 0 5px;
        .accept-consent-btn {
            display: flex;
            height: 48px;
            color: @color-white;
            background-color: @BtnBackground!important;
            border-radius: 4px;
        }
    }

</style>