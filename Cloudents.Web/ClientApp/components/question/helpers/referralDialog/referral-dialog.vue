<template>
    <v-card elevation="0" :style="isTransparent ? 'background: transparent;' : 'background-color: #fff;' ">
        <div class="dialog-wrapp referral-container">
            <button class="close-btn text-md-right" @click.prevent="requestDialogClose()">
                <v-icon>sbf-close</v-icon>
            </button>
            <div class="ml-1 wrap-text input-container">
                <h2 class="text-md-left" v-html="text.dialog.title"></h2>
                <div class="link-container">
                    <sb-input id="sb_referralLink" class="referral-input" :disabled="true" v-model="userReferralLink"
                              name="referralLink" type="text" :prependInnerIcon="'sbf-share-icon'"></sb-input>                    &nbsp;
                    <button class="referral-btn" :class="{'copied': isCopied}" @click="doCopy">
                        <v-icon class="copy-check-icon" transition="fade-transition"
                                v-show="isCopied">sbf-checkmark</v-icon>
                       <span v-show="!isCopied" v-language:inner>referralDialog_copy</span>
                        <span v-show="isCopied" v-language:inner>referralDialog_copied</span>
                    </button>
                </div>
            </div>
            <div class="text-style-wrap" style="margin-bottom: 20px;">
                <span class="text-style" v-language:inner>referralDialog_share_with</span>
            </div>
            <div class="share-icon-container">
                <span @click="shareOnSocialMedia(socialMedias.whatsApp)">
                    <v-icon class="referral-whatsup-icon">sbf-whats-up</v-icon>
                                   </span>
                <span @click="shareOnSocialMedia(socialMedias.facebook)">
                    <v-icon class="referral-facebook-icon">sbf-facebook-share</v-icon>
                </span>
                <span @click="shareOnSocialMedia(socialMedias.linkedin)">
                    <v-icon  class="referral-linkedin-icon">sbf-linkedin-share</v-icon>

                </span>
                <span @click="shareOnSocialMedia(socialMedias.twitter)">
                    <v-icon class="referral-twitter-icon">sbf-twitter-share</v-icon>
                </span>
                <span @click="shareOnSocialMedia(socialMedias.gmail)">
                    <v-icon class="referral-gmail-icon">sbf-google-icon</v-icon>
                </span>
            </div>
             </div>
    </v-card>
</template>

<script>
    import SbInput from "../sbInput/sbInput.vue"
    import { mapGetters } from "vuex"
    import Base62 from "base62"
    import { LanguageService } from '../../../../services/language/languageService'
    import {getReferallMessages}  from "./consts.js";

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
                text: {
                    dialog: {
                        title: LanguageService.getValueByKey("referralDialog_dialog_title_invite")
                    }
                }
            }
        },
        props: {
            referralType:{
                type:String,
                default: 'menu',
                required: false
            },
            userReferralLink:{
                type: String,
                // http://www.spitball.co/" +"?referral=" + Base62.encode(this.accountUser().id) + "&promo=referral
                default: "",
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
            closeDialog:{
                required:false,
                type: Function
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
            ...mapGetters(['accountUser']),
            requestDialogClose() {
                this.isCopied = false;
                this.$root.$emit('closePopUp', this.popUpType);
                if(this.closeDialog){
                    this.closeDialog()
                }
            },
            doCopy() {
                let self = this;
                this.$copyText(this.userReferralLink).then((e) => {
                    self.isCopied = true;
                }, (e) => {
                })
            },
            shareOnSocialMedia(socialMedia) {
                let message = getReferallMessages(this.referralType, this.userReferralLink);
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
        beforeDestroy() {
            this.isCopied = false;
        }
    }
</script>


<style src="./referral-dialog.less" lang="less"></style>
