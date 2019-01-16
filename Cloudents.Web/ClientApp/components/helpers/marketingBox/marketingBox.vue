<template>
    <div class="marketing-box-component">
        <div class="heading" v-if="$vuetify.breakpoint.smAndDown">
            <span class="heading-text">Promotions</span>
        </div>
        <v-card class="main-marketing-content"  :style="{ 'background-image': 'url(' + require(`${imgSrc}`) + ')' }" @click="promotionOpen()">
        </v-card>
        <sb-dialog :showDialog="showReferral"
                   :popUpType="'referralPop'"
                   :content-class="'login-popup'"
                   :onclosefn="closeRefDialog">
            <referral-dialog :closeDialog="closeRefDialog" :isTransparent="false" :userReferralLink="userReferralLink"
                             :popUpType="'referralPop'"></referral-dialog>
        </sb-dialog>

    </div>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import referralDialog from "../../question/helpers/referralDialog/referral-dialog.vue";
    import Base62 from "base62"
    import SbDialog from "../../wrappers/sb-dialog/sb-dialog.vue"

    export default {
        name: "marketingBox",
        components: {referralDialog, SbDialog},
        data() {
            return {
                showReferral: false,
                desktop: {
                    hebrew: {
                        logedIn: './images/desktop_Hebrew_LogedIn.png',
                        not_logedIn: './images/desktop_Hebrew_Not_LogedIn.png',
                    },
                    english: {
                        logedIn: './images/desktop_English_LogedIn.png',
                        not_LogedIn: './images/desktop_English_Not_LogedIn.png',
                    }
                },
                mobile: {
                    hebrew: {
                        logedIn: './images/mobile_Hebrew_LogedIn.png',
                        not_logedIn: './images/mobile_Hebrew_Not_LogedIn.png',
                    },
                    english: {
                        logedIn: './images/mobile_English_LogedIn.png',
                        not_logedIn: './images/mobile_English_Not_LogedIn.png',
                    }
                }
            }
        },
        computed: {
            ...mapGetters(['accountUser']),
            isIsrael() {
                return global.isIsrael;
            },
            isMobile() {
                return this.$vuetify.breakpoint.xsOnly
            },
            isLogedIn() {
                return (this.accountUser !=null)
            },
            imgSrc() {
                let imageSrc = '';
                let imagesSet = this.$vuetify.breakpoint.xsOnly ? this.mobile : this.desktop;
                imagesSet = this.isIsrael ? imagesSet.hebrew : imagesSet.english;
                imageSrc = this.isLogedIn ? imagesSet["logedIn"] : imagesSet["not_logedIn"];
                return imageSrc
            },
            userReferralLink() {
                if (!this.isLogedIn) {
                    return `${global.location.origin}` + "?promo=referral";
                } else {
                    return `${global.location.origin}` + "?referral=" + Base62.encode(this.accountUser.id) + "&promo=referral";
                }
            },


        },
        methods: {
            ...mapActions(['changemobileMarketingBoxState']),
            closeRefDialog() {
                this.showReferral = false
            },
            promotionOpen() {
                return this.isLogedIn ? this.openRefDialog() : this.goToRegister();
            },
            goToRegister(){
                this.changemobileMarketingBoxState();
                this.$router.push({name: 'registration'});
            },
            openRefDialog() {
                this.showReferral = true
            }

        },
    }
</script>

<style src="./marketingBox.less" lang="less">

</style>