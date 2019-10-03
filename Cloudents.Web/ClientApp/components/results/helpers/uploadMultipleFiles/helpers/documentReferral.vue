<template>
    <v-card elevation="0" class="mb-3 sb-step-card ref-card" :class="{'mb-5': $vuetify.breakpoint.smAndUp}">
        <v-flex align-center justify-center>
            <div v-if="fileSnackbar.visibility"  class="file-snack-bar px-5" :style="{ backgroundColor: fileSnackbar.color}">
                <span class="file-snack-text" style="">
                    <span>
                        <v-icon class="file-snack-icon">sbf-checkmark</v-icon>
                    </span>
                    <span>{{fileSnackbar.uploadDoneMessage ? fileSnackbar.uploadDoneMessage : ''}}</span>
                </span>
            </div>
        </v-flex>
        <v-layout justify-center align-center column class="mt-3 px-5">
            <v-flex column class="ref-title justify-center align-center flex mt-2 row"
                    :class="{'column': $vuetify.breakpoint.xsOnly}">
                <i class="five mb-3">
                    <spreadOutLoudIcon style="width: 50px"></spreadOutLoudIcon>
                </i>
                <h3 class="sb-title text-xs-center" v-language:inner>upload_multiple_files_referral_title</h3>

            </v-flex>
            <v-flex class="ref-title justify-center align-center flex mt-2">
                <span class="text-xs-center" v-language:inner>upload_multiple_help_and_make</span>
            </v-flex>
        </v-layout>

        <div class="upload-row-2 referral-row refs-container">
            <referral-dialog class="mb-4 referral-item"
                             :isMultiple="referralLinks.length > 1"
                             :userReferralLink="singleRefLink"
                             :referralType="'uploadReffer'"
                             :refLinkArr="referralLinks"
                             :isTransparent="true"
                             :popUpType="''"
            ></referral-dialog>
        </div>
    </v-card>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import referralDialog from "../../../../question/helpers/referralDialog/referral-dialog.vue";
    import spreadOutLoudIcon from '../../../../../font-icon/spread-loud.svg';

    export default {

        name: "uploadStep_3",
        components: {referralDialog, spreadOutLoudIcon},
        data() {
            return {}
        },
        props: {
            referralLinks: {
                type: Array,
                default: [],
            },
            fileSnackbar:{
                type: Object,
                required:false
            }

        },
        computed: {
            ...mapGetters({
                getFileData: 'getFileData',
                getSchoolName: 'getSchoolName',
            }),
            singleRefLink() {
                if (this.referralLinks && this.referralLinks.length === 1) {
                    return this.referralLinks[0].itemRefLink
                }
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
            goToaskQuestion() {
                this.$router.push({path: '/ask'});
                this.updateDialogState(false)
            }
        }
    }
</script>

<style lang="less">
    @import "../../../../../styles/mixin.less";

    .ref-card {
        .sb-title{
            font-size: 18px;
        }
        .file-snack-bar{
            display: flex;
            width: 100%;
            align-items: center;
            justify-content: center;
            box-shadow: 0 4px 7px 0 rgba(0, 0, 0, 0.34);
            .file-snack-text{
                color: fade(@color-white, 87%);
                padding: 16px 0;
                text-align: center;
                font-size: 14px;
                line-height: 1.43;
                letter-spacing: -0.1px;
                display: flex;
                align-items: baseline;

                .file-snack-icon{
                    font-size: 22px;
                    color: @color-white;
                }
            }

        }
        .ref-title {
            display: flex;
            &.column {
                flex-direction: column;
            }
            .five {
                @media (max-width: @screen-xs) {
                    margin-top: 24px;
                    margin-bottom: 16px;
                }
            }
        }
        .ref-subtitle {
            font-size: 14px;
            letter-spacing: -0.6px;
            color: @colorBlackNew;
        }
        .referral-row {
            margin-top: 54px;
            overflow: hidden;//edge fix
            &.refs-container {
                max-height: 300px;
                @media (max-width: @screen-xs) {
                    max-height: unset;
                    padding-left: 12px;
                    padding-right: 12px;
                }
            }
            .v-card {
                box-shadow: none;
                @media (max-width: @screen-xs) {
                    width: 100%;
                }
                .close-btn, h2 {
                    display: none;
                }
                .referral-container {
                    display: flex;
                    flex-direction: column;
                    background-color: transparent;
                    background: transparent;
                    .share-icon-container {
                        max-width: 346px;
                        order: 2;
                        @media (max-width: @screen-xs) {
                            display: flex;
                            flex-direction: row;
                            width: 100%;
                            justify-content: space-around;
                        }
                        .share-icon{
                            /*rtl:ignore*/
                            direction: ltr;


                        }
                    }
                    .input-container {
                        width: 100%;
                        margin: unset;
                        margin-left: 0;
                        order: 1;
                        @media (max-width: @screen-xs) {
                            margin-left: 0 !important;
                        }
                        .link-container {
                            width: 100%;
                            margin-top: 0;
                            max-width: 346px;
                            .referral-input {
                                min-width: unset;
                                @media (max-width: @screen-xs) {
                                    min-width: unset;
                                }
                            }
                        }
                    }

                    .text-style-wrap {
                        display: none;
                    }
                }
            }
        }
    }
</style>