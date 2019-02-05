<template>
    <v-card elevation="0" class="mb-3 sb-step-card ref-card" :class="{'mt-5': $vuetify.breakpoint.smAndUp}">
        <v-layout justify-center align-center column >
            <v-flex class="ref-title justify-center align-center flex mt-2 row" :class="{'column': $vuetify.breakpoint.xsOnly}">
                    <i class="five">
                        <spreadOutLoudIcon style="width: 50px"></spreadOutLoudIcon>
                    </i>
                    <h3 class="sb-title" v-language:inner>upload_multiple_files_referral_title</h3>

            </v-flex>
            <v-flex  class="ref-title justify-center align-center flex mt-2">
                <span>Help your friends and make money along the way </span>
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


        },
        computed: {
            ...mapGetters({
                getFileData: 'getFileData',
                getSchoolName: 'getSchoolName',
            }),
            singleRefLink(){
                if( this.referralLinks &&  this.referralLinks.length ===1){
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
        .ref-title{
            display: flex;
            &.column{
                flex-direction: column;
            }
            .five{
                @media (max-width: @screen-xs) {
                    margin-top: 24px;
                    margin-bottom: 16px;
                }
            }
        }
        .ref-subtitle{
            font-family: @fontOpenSans;
            font-size: 14px;
            letter-spacing: -0.6px;
            color: @colorBlackNew;
        }
        .referral-row {
            overflow-y: scroll;
            padding-top: 21px;
            &.refs-container{
                max-height: 300px;
                @media (max-width: @screen-xs) {
                   max-height: unset;
                    padding-left: 12px;
                    padding-right: 12px;
                }
                .referral-item{
                    &:last-child{
                        margin-bottom: 72px!important;
                    }
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
                    .share-icon-container {
                        span {
                            margin-right: 24px;
                        }
                    }
                    .input-container {
                        width: 100%;
                        @media (max-width: @screen-xs) {
                            margin-left: 0 !important;
                        }
                        .link-container {
                            width: 100%;
                            margin-top: 0;
                            .referral-input {
                                min-width: 320px;
                                @media (max-width: @screen-xs) {
                                    min-width: unset;
                                }
                            }
                        }
                    }
                    .share-icon-container {
                        @media (max-width: @screen-xs) {
                            display: flex;
                            flex-direction: row;
                            width: 80%;
                            //justify-content: space-evenly;
                            justify-content: space-around;
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