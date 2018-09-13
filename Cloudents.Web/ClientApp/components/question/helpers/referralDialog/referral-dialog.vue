<template>
    <v-card>
        <div class="dialog-wrapp referral-container">
            <button class="close-btn text-md-right" @click.prevent="requestDialogClose()">
                <v-icon>sbf-close</v-icon>
            </button>
            <div class="ml-1 wrap-text input-container">
                <h2 class="text-md-left">Invite a friend and Earnd for each referral 10 SBL tokens.</h2>
                <div class="wrapper-body-text link-container">
                    <sb-input id="sb_referralLink" class="referral-input" :disabled="true" v-model="userReferralLink" name="referralLink" type="text"></sb-input>
                    &nbsp;
                    <button @click="copyStringToClipboard()" class="btn">Copy</button>
                </div>
            </div>
            <div style="margin-bottom: 20px;">
                <span class="text-style">or share with</span>
            </div>
            <div class="share-icon-container">
                <v-icon @click="shareOnSocialMedia(socialMedias.whatsApp)">sbf-share-whatsapp</v-icon>
                <v-icon @click="shareOnSocialMedia(socialMedias.facebook)">sbf-share-facebook</v-icon>
                <v-icon @click="shareOnSocialMedia(socialMedias.twitter)">sbf-share-twitter</v-icon>
                <v-icon @click="shareOnSocialMedia(socialMedias.linkedin)">sbf-share-linkedin</v-icon>
                <v-icon @click="shareOnSocialMedia(socialMedias.gmail)">sbf-share-email</v-icon>
            </div>
        </div>
    </v-card>
</template>

<script>
import SbInput from "../sbInput/sbInput.vue";
export default {
    components:{SbInput},
    data(){
        return {
            userReferralLink: "http://www.spitball.co/referral=XxvbzJhys45s",
            socialMedias: {
                whatsApp: "whatsApp",
                facebook: "facebook",
                linkedin: "linkedin",
                twitter: "twitter",
                gmail: "gmail",
            }
        }
    },
    props:{
        popUpType:{
            type:String,
            required: true
        }
    },
    methods: {
        requestDialogClose() {
            this.$root.$emit('closePopUp', this.popUpType);
        },
       copyStringToClipboard () {
            // Create new element
            var el = document.createElement('textarea');
            // Set value (string to be copied)
            el.value = this.userReferralLink;
            // Set non-editable to avoid focus and move outside of view
            el.setAttribute('readonly', '');
            el.style = {position: 'absolute', left: '-9999px'};
            document.body.appendChild(el);
            // Select text inside element
            el.select();
            // Copy text to clipboard
            document.execCommand('copy');
            // Remove temporary element
            document.body.removeChild(el);
        },
        shareOnSocialMedia(socialMedia){
            let message = {
                url: this.userReferralLink,
                title: `Ask and answer with Spitball`,
                text: `I ask and answer with Spitball! Get free 100SBL if you use this link. ${this.userReferralLink}`,
                twitterText: `I ask and answer with Spitball! Get free 100SBL if you use this link.`,
            };

            switch(socialMedia){
                case this.socialMedias.whatsApp:
                //https://api.whatsapp.com/send?text={{url  here}}
                    global.open(`https://api.whatsapp.com/send?text=${message.text}`,"_blank")
                break;
                case this.socialMedias.facebook:
                //https://www.facebook.com/sharer.php?u={{url  here}}
                    global.open(`https://www.facebook.com/sharer.php?u=${message.url}`,"_blank")
                break;
                case this.socialMedias.linkedin:
                //https://www.linkedin.com/shareArticle?mini=true&url={{url here}}&title={{title here}}&summary={{text here}}
                global.open(`https://www.linkedin.com/shareArticle?mini=true&url=${message.url}&title=${message.title}&summary=${message.text}`,"_blank")
                break;
                case this.socialMedias.twitter:
                //https://twitter.com/intent/tweet?url={{url here}}&text={{text here}}&hashtags={{hashtag name}}
                    global.open(`https://twitter.com/intent/tweet?url=${message.url}&text=${message.twitterText}`,"_blank")
                break;
                case this.socialMedias.gmail:
                //https://mail.google.com/mail/?view=cm&su={{title here}}&body={{url here}}
                global.open(`https://mail.google.com/mail/?view=cm&su=${message.title}&body=${message.text}`,"_blank")
                break;
            }
        }            
    },
}
</script>

<style lang="less" scoped>
    .referral-container{
        text-align: center;
        vertical-align: middle;
        align-content: center;
        align-items: center;
        display: flex;
        flex-direction: inherit;
        .input-container{
            display: flex;
            flex-direction: column;
            align-items: center;
            vertical-align: middle;
            margin: 0 auto;
            .link-container{
                margin-top:30px;
            }
        }
        .text-style{
            color: #3532d5;
            font-weight: 600;
        }
        .share-icon-container{
            width: 35%;
            display: flex;
            justify-content: space-evenly;
        }
    }
    .btn{
        border-radius: 4px;
        background-color: #3532d5;
        color: #fff;
        font-size: 20px;
        font-weight: bold;
        line-height: 40px;
        letter-spacing: -0.5px;
        width: 100%;
        display: block;
        text-align: center;
        margin-bottom: 17px;
        -webkit-transition: all 0.2s ease-in-out;
    }
    .referral-input{
        min-width: 300px;
        color:gray;
        background: #e5e5e5;
    }

    
</style>
