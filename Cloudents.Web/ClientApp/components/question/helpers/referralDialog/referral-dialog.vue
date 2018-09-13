<template>
    <v-card>
        <div class="dialog-wrapp referral-container">
            <button class="close-btn text-md-right" @click.prevent="requestDialogClose()">
                <v-icon>sbf-close</v-icon>
            </button>
            <div class="ml-1 wrap-text input-container">
                <h2 class="text-md-left"> <b>Invite a friend</b> and <b>Earn</b> for each referral <b>10 SBL tokens.</b></h2>
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
                <span @click="shareOnSocialMedia(socialMedias.whatsApp)">
                    <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 32 32">
                        <g fill="none" fill-rule="nonzero">
                            <path fill="#4CAF50" d="M16.004 0h-.008C7.174 0 0 7.176 0 16c0 3.5 1.128 6.744 3.046 9.378l-1.994 5.944 6.15-1.966A15.862 15.862 0 0 0 16.004 32C24.826 32 32 24.822 32 16S24.826 0 16.004 0z"/>
                            <path fill="#FAFAFA" d="M25.314 22.594c-.386 1.09-1.918 1.994-3.14 2.258-.836.178-1.928.32-5.604-1.204-4.702-1.948-7.73-6.726-7.966-7.036-.226-.31-1.9-2.53-1.9-4.826S7.87 8.372 8.34 7.892c.386-.394 1.024-.574 1.636-.574.198 0 .376.01.536.018.47.02.706.048 1.016.79.386.93 1.326 3.226 1.438 3.462.114.236.228.556.068.866-.15.32-.282.462-.518.734-.236.272-.46.48-.696.772-.216.254-.46.526-.188.996.272.46 1.212 1.994 2.596 3.226 1.786 1.59 3.234 2.098 3.752 2.314.386.16.846.122 1.128-.178.358-.386.8-1.026 1.25-1.656.32-.452.724-.508 1.148-.348.432.15 2.718 1.28 3.188 1.514.47.236.78.348.894.546.112.198.112 1.128-.274 2.22z"/>
                        </g>
                    </svg>
                </span>
                <span>
                    <svg @click="shareOnSocialMedia(socialMedias.facebook)" xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 32 32">
                        <g fill="none" fill-rule="nonzero">
                            <circle cx="15.886" cy="15.886" r="15.886" fill="#3B5998"/>
                            <path fill="#FFF" d="M19.88 16.508h-2.835v10.385H12.75V16.508h-2.042v-3.65h2.042v-2.361c0-1.69.803-4.334 4.333-4.334l3.182.013V9.72h-2.308c-.38 0-.911.19-.911.995v2.148h3.21l-.376 3.646z"/>
                        </g>
                    </svg>
                </span>
                <span @click="shareOnSocialMedia(socialMedias.linkedin)">
                    <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 32 32">
                        <g fill="none" fill-rule="nonzero">
                            <ellipse cx="15.886" cy="16.027" fill="#007AB9" rx="15.886" ry="15.747"/>
                            <path fill="#F1F2F2" d="M25.378 17.294v6.492h-3.797V17.73c0-1.52-.549-2.559-1.923-2.559-1.05 0-1.673.7-1.948 1.376-.1.242-.126.578-.126.918v6.322h-3.797s.05-10.258 0-11.32h3.797v1.604c-.007.013-.018.025-.025.037h.025v-.037c.505-.77 1.405-1.87 3.423-1.87 2.498 0 4.371 1.618 4.371 5.094zM9.814 7.01c-1.299 0-2.149.845-2.149 1.955 0 1.087.825 1.957 2.1 1.957h.024c1.324 0 2.148-.87 2.148-1.957-.025-1.11-.824-1.955-2.123-1.955zM7.891 23.786h3.796v-11.32H7.891v11.32z"/>
                        </g>
                    </svg>
                </span>
                <span @click="shareOnSocialMedia(socialMedias.twitter)">
                    <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 32 32">
                        <g fill="none" fill-rule="nonzero">
                            <circle cx="15.886" cy="15.886" r="15.886" fill="#55ACEE"/>
                            <path fill="#F1F2F2" d="M25.617 11.417a7.576 7.576 0 0 1-2.18.597 3.808 3.808 0 0 0 1.67-2.1 7.602 7.602 0 0 1-2.412.921 3.798 3.798 0 0 0-6.47 3.464 10.78 10.78 0 0 1-7.828-3.968 3.797 3.797 0 0 0 1.175 5.07 3.77 3.77 0 0 1-1.72-.476v.048a3.8 3.8 0 0 0 3.046 3.723 3.784 3.784 0 0 1-1.715.066 3.801 3.801 0 0 0 3.548 2.637 7.619 7.619 0 0 1-5.622 1.572 10.74 10.74 0 0 0 5.82 1.707c6.985 0 10.804-5.786 10.804-10.805 0-.164-.003-.328-.01-.49a7.7 7.7 0 0 0 1.894-1.966z"/>
                        </g>
                    </svg>
                </span>
                <span @click="shareOnSocialMedia(socialMedias.gmail)">
                    <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 32 32">
                        <g fill="none" fill-rule="nonzero">
                            <circle cx="16" cy="16" r="16" fill="#FF2800"/>
                            <path fill="#EA0A0A" d="M25.988 9.217H5.972l7.252 7.55 2.092 2.119c-1.138-.26-1.209-.423-1.713-.835-.265-.216-.776-.803-1.553-1.596L5.514 9.78v12.427l.516.516-.058.06 9.195 9.196c.276.014.554.021.833.021 8.837 0 16-7.163 16-16 0-.265-.007-.53-.02-.791l-5.992-5.992z"/>
                            <g fill="#FFF">
                                <path d="M5.514 9.78v12.427l6.06-6.239zM26.486 9.78v12.427l-6.03-6.214zM5.972 9.217h20.016l-7.866 8.19a2.97 2.97 0 0 1-4.283 0l-7.867-8.19z"/>
                                <path d="M18.576 17.843a3.616 3.616 0 0 1-2.596 1.106c-.974 0-1.92-.403-2.595-1.105l-1.334-1.39-6.079 6.33h20.016l-6.078-6.33-1.334 1.39z"/>
                            </g>
                        </g>
                    </svg>
                </span>
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


<style src="./referral-dialog.less" lang="less"></style>
