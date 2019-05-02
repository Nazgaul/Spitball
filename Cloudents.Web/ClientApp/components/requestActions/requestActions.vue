<template>
    <div class="request-box-wrap">
        <v-layout>
            <v-flex xs12 sm12 md12 class="text-xs-center pt-4">
                <span class="subheading font-weight-bold request-box-title" v-language:inner>requestActions_title
                </span>
            </v-flex>
        </v-layout>
        <v-layout align-center class="pt-3 pb-3">
            <v-flex sm4 md4 class="btn-wrap text-xs-center">
                <v-btn round class="light-btn px-4 elevation-0" @click="openAskQuestion()">
                    <v-icon class="light-btn-icon subheading mr-2">sbf-message-icon-new</v-icon>
                    <span v-show="$vuetify.breakpoint.smAndUp" class="text-capitalize body-2 font-weight-bold"
                          v-language:inner>requestActions_btn_ask</span>
                    <span v-show="$vuetify.breakpoint.xsOnly" class="text-capitalize body-2 font-weight-bold"
                          v-language:inner>requestActions_btn_ask_mob</span>
                </v-btn>
            </v-flex>
            <v-flex sm4 md4 class="btn-wrap text-xs-center">
                <v-btn round class="light-btn px-4 elevation-0" @click="openUpload()">
                    <v-icon class="light-btn-icon subheading mr-2">sbf-upload-icon</v-icon>
                    <span v-show="$vuetify.breakpoint.smAndUp" class="text-capitalize body-2 font-weight-bold"
                          v-language:inner>requestActions_btn_upload</span>
                    <span v-show="$vuetify.breakpoint.xsOnly" class="text-capitalize body-2 font-weight-bold"
                          v-language:inner>requestActions_btn_upload_mob</span>
                </v-btn>
            </v-flex>
            <v-flex sm4 md4 class="btn-wrap text-xs-center">
                <v-btn round class="light-btn px-4 elevation-0" @click="openRequestTutor()">
                    <v-icon class="light-btn-icon subheading mr-2">sbf-person-icon</v-icon>
                    <span v-show="$vuetify.breakpoint.smAndUp" class="text-capitalize body-2 font-weight-bold"
                          v-language:inner>requestActions_btn_tutor</span>
                    <span v-show="$vuetify.breakpoint.xsOnly" class="text-capitalize body-2 font-weight-bold"
                          v-language:inner>requestActions_btn_tutor_mob</span>
                </v-btn>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    export default {
        name: "requestActions",
        computed: {
            ...mapGetters(['accountUser', 'getSchoolName', 'getSelectedClasses']),
            name() {
                return this.data;
            }
        },
        methods: {
            ...mapActions([
                              'updateNewQuestionDialogState',
                              'updateLoginDialogState',
                              'updateUserProfileData',
                              'setReturnToUpload',
                              'updateDialogState',
                              'updateRequestDialog'
                          ]),
            openAskQuestion() {
                if(this.accountUser == null) {
                    this.updateLoginDialogState(true);
                    //set user profile
                    this.updateUserProfileData('profileHWH');
                } else {
                    this.updateNewQuestionDialogState(true);
                }

            },
            openUpload() {
                let schoolName = this.getSchoolName;
                if(this.accountUser == null) {
                    this.updateLoginDialogState(true);
                } else if(!schoolName.length) {
                    this.$router.push({name: 'addUniversity'});
                    this.setReturnToUpload(true);
                } else if(!this.getSelectedClasses.length) {
                    this.$router.push({name: 'addCourse'});
                    this.setReturnToUpload(true);
                } else if(schoolName.length > 0 && this.getSelectedClasses.length > 0) {
                    this.updateDialogState(true);
                    this.setReturnToUpload(false);
                }
            },
            openRequestTutor() {
                if(this.accountUser == null) {
                    this.updateLoginDialogState(true);
                } else {
                    this.updateRequestDialog(true)
                }
                console.log('open tutor request dialog')
            }
        },
    };
</script>

<style lang="less">
    @import '../../styles/mixin.less';

    .request-box-wrap {
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24);
        background-color: @color-white;
        border-radius: 4px;
        .request-box-title {
            color: @textColor;
            letter-spacing: -0.4px;
        }
        .light-btn {
            border: solid 1px @lightBtnBorderColor;
            border-radius: 16px;
            height: 32px;
            color: @textColor;
            width: 189px;
            background: @lightBtnColor !important;
            @media (max-width: @screen-xs) {
                width: 110px;
            }
            .light-btn-icon {
                color: @lightBtnIconColor;
            }
        }

    }

</style>