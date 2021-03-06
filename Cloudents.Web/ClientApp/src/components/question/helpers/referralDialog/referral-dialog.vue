<template>
    <v-card class="ref-block" elevation="0"
            :style="isTransparent ? 'backgroundColor: transparent;' : 'background-color: #fff;' ">
        <div class="dialog-wrapp flex-grow-1 referral-container">
            <v-icon class="close-btn-referral body-1 pe-3 pt-3" @click.prevent="requestDialogClose()">sbf-close</v-icon>
            <v-layout column v-show="!isUploadReferral">
                <v-flex xs12 class="mb-4">
                    <div>
                        <span class="ref-title" v-html="text.dialog.titleSpread"></span>
                    </div>
                </v-flex>
                <v-flex xs12 class="mb-4">
                    <div>
                        <span class="text-center ref-subtitle" v-html="text.dialog.subTitle"></span>
                    </div>
                </v-flex>
            </v-layout>

            <v-layout align-center justify-center column class="mb-2">
                <div class="share-icon-container">
                    <span @click="shareOnSocialMedia(socialMedias.facebook)" class="share-btn facebook-share-btn">
                        <v-icon class="share-icon">sbf-facebook-share</v-icon>
                        <span class="share-text" v-t="'referralDialog_share_facebook'"></span>
                    </span>
                    <span @click="shareOnSocialMedia(socialMedias.twitter)" class="share-btn twitter-share-btn">
                        <v-icon class="share-icon referral-twitter-icon">sbf-tweeter-share</v-icon>
                        <span class="share-text referral-twitter" v-t="'referralDialog_share_tweeter'"></span>
                    </span>
                    <span @click="shareOnSocialMedia(socialMedias.gmail)" class="share-btn gmail-share-btn">
                        <v-icon class="share-icon referral-gmail-icon">sbf-google-share</v-icon>
                        <span class="share-text" v-t="'referralDialog_share_google'"></span>
                    </span>
                    <span @click="shareOnSocialMedia(socialMedias.whatsApp)" class="share-btn whatsup-share-btn">
                        <v-icon class="share-icon">sbf-whatsup-share</v-icon>
                        <span class="share-text" v-t="'referralDialog_share_whatsup'"></span>
                    </span>
                </div>
                <div class="input-container mb-4">
                    <div class="link-container">
                        <sb-input ref="inputCopy" v-if="!isMultiple" id="sb_referralLink" class="referral-input" :disabled="true"
                                  v-model="userReferralLink"
                                  name="referralLink" type="text" :prependInnerIcon="'sbf-share-icon'"></sb-input>
                        <v-select
                                v-else
                                :items="refLinkArr"
                                v-model="singleRefLink"
                                item-value="itemRefLink"
                                item-text="itemName"
                                @change="clearCopied()"
                                class="sb-field select-referral elevation-0"
                                hide-details
                                :prepend-icon="''"
                                :placeholder="referralSelectPlace"
                                :append-icon="'sbf-arrow-down'"
                                solo
                                single-line
                        ></v-select>
                        <button :disabled="isMultiple && !singleRefLink" class="referral-btn"
                                :class="{'copied': isCopied}"
                                @click="doCopy">
                            <v-icon class="copy-check-icon" transition="fade-transition"
                                    v-show="isCopied">sbf-checkmark
                            </v-icon>
                            <span v-show="!isCopied && !isMultiple" v-t="'referralDialog_copy'"></span>
                            <span v-show="!isCopied && isMultiple" v-t="'referralDialog_copy_link'"></span>
                            <span v-show="isCopied" v-t="'referralDialog_copied'"></span>
                        </button>

                    </div>
                </div>
            </v-layout>
            <v-layout align-center justify-center class="mb-2" v-show="!isUploadReferral">
                <v-flex xs12>
                    <div>
                        <div class="bottom-sub" v-html="text.dialog.bottomText"></div>
                        <div class="bottom-sub bottom-sub-small" v-html="text.dialog.bottomTextSm"></div>
                        <a class="bottom-sub termsLink" :href="$t('referralDialog_terms_link')" target="_blank" v-if="termOfServiceLink">
                            {{$t('referralDialog_terms_link_text')}}
                        </a>
                    </div>
                </v-flex>
            </v-layout>
        </div>
        <v-layout align-center class="ref-bottom-section px-3 flex-grow-0" v-show="!isUploadReferral">
            <v-flex xs1 >
                <!-- <i class="bottom-five"> -->
                    <v-icon  class="spread-out-loud-icon">sbf-spread-loud</v-icon>
                <!-- </i> -->
            </v-flex>
            <v-flex xs11 >
                <div style="text-align: center">
                    <span class="bottom-text joined-number">{{usersReffered}}</span>
                    <span class="bottom-text" v-html="text.dialog.friendsJoined"></span>
                </div>
            </v-flex>

        </v-layout>
    </v-card>
</template>

<script>
    import SbInput from "../sbInput/sbInput.vue"
    import { mapGetters, mapActions } from "vuex"
    import Base62 from "base62"
    import { getReferallMessages } from "./consts.js";

    export default {
        components: {SbInput},
        data() {
            return {

                socialMedias: {
                    whatsApp: "whatsApp",
                    facebook: "facebook",
                    linkedin: "linkedin",
                    twitter: "twitter",
                    gmail: "gmail",
                },
                isCopied: false,
                singleRefLink: '',
                referralSelectPlace: this.$t("referralDialog_ref_placeholder"),
                text: {
                    dialog: {
                        title: this.$t("referralDialog_dialog_title_invite"),
                        titleSpread: this.$t("referralDialog_spread"),
                        subTitle: this.$t("referralDialog_dialog_subtitle"),
                        bottomText: this.$t("referralDialog_dialog_bottom_text"),
                        bottomTextSm: '', // this.$t("referralDialog_dialog_bottom_text2"),
                        friendsJoined: this.$t("referralDialog_dialog_friends_joined"),
                    }
                }
            }
        },

        props: {
            referralType: {
                type: String,
                default: 'menu',
                required: false
            },
            isMultiple: {
                type: Boolean,
                required: false
            },
            refLinkArr: {
                type: Array,
                required: false
            },
            popUpType: {
                type: String,
                required: true
            },
            showDialog: {
                type: Boolean,
                default: false
            },
            isTransparent: {
                type: Boolean,
                default: false,
                required: false
            },
            onclosefn: {
                required: false,
            }
        },
        computed: {
            ...mapGetters(['isFrymo','usersReffered', 'accountUser']),
            isUploadReferral() {
                return this.referralType === 'uploadReffer'; //TODO: check if we need this 'uploadReffer'
            },
            userReferralLink() {
            let site = this.isFrymo ? 'frymo.com' : 'spitball.co';
            return `http://www.${site}/?referral=${Base62.encode(this.accountUser.id)}&promo=referral`;
            },
            termOfServiceLink() {
                return global.country === 'US';

            }
        },
        watch: {
            showDialog() {
                if (!this.showDialog) {
                    this.isCopied = false;
                }
            },
        },
        methods: {
            ...mapActions(['getRefferedUsersNum']),
            requestDialogClose() {
                this.isCopied = false;
                if(this.onclosefn) this.onclosefn()

            },
            clearCopied() {
                if (this.isCopied) {
                    this.isCopied = false;
                }
            },
            doCopy() {
                let link;
                if (!this.isMultiple) {
                    link = this.userReferralLink;
                }else{
                    link = this.singleRefLink;
                }
                let self = this;
                this.$copyText(link,this.$refs.inputCopy.$el).then(() => {
                    self.isCopied = true;
                    }, () => {
                })
            },
            shareOnSocialMedia(socialMedia) {
                let message = getReferallMessages(this.referralType, this.isMultiple ? this.singleRefLink : this.userReferralLink);
                switch (socialMedia) {
                    case this.socialMedias.whatsApp:
                        //https://api.whatsapp.com/send?text={{url  here}}
                        global.open(`https://web.whatsapp.com/send?text=${message.whatsAppText}`, "_blank");
                        break;
                    case this.socialMedias.facebook:
                        //https://www.facebook.com/sharer.php?u={{url  here}}
                        global.open(`https://www.facebook.com/sharer.php?u=${message.encodedUrl}`, "_blank");
                        break;
                    case this.socialMedias.linkedin:
                        //https://www.linkedin.com/shareArticle?mini=true&url={{url here}}&title={{title here}}&summary={{text here}}
                        global.open(`https://www.linkedin.com/shareArticle?mini=true&url=${message.encodedUrl}&title=${message.title}&summary=${message.text}`, "_blank");
                        break;
                    case this.socialMedias.twitter:
                        //https://twitter.com/intent/tweet?url={{url here}}&text={{text here}}&hashtags={{hashtag name}}
                        global.open(`https://twitter.com/intent/tweet?url=${message.encodedUrl}&text=${message.twitterText}`, "_blank");
                        break;
                    case this.socialMedias.gmail:
                        //https://mail.google.com/mail/?view=cm&su={{title here}}&body={{url here}}
                        global.open(`https://mail.google.com/mail/?view=cm&su=${message.title}&body=${message.text}`, "_blank");
                        break;
                }
            }
        },
        created() {
            if(this.referralType != 'uploadReffer'){
                this.getRefferedUsersNum()
            }
        },

        beforeDestroy() {
            this.isCopied = false;
        }
    }
</script>


<style src="./referral-dialog.less" lang="less"></style>
