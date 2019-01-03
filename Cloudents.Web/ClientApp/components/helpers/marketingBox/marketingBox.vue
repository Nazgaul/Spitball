<template>
    <div class="marketing-box-component">
        <div class="heading">
            <span class="heading-text">Promotions</span>
        </div>
        <v-card class="main-marketing-content">
            <img :src="require(`${imgSrc}`)" alt="" @click="relevantAction()">
        </v-card>
        <referral-dialog :isTransparent="false" :showDialog="showReferral" :userReferralLink="userReferralLink" :popUpType="'referralPop'"></referral-dialog>

    </div>
</template>

<script>
    import { mapGetters } from 'vuex';
    import referralDialog from "../../question/helpers/referralDialog/referral-dialog.vue";
    import Base62 from "base62"

    export default {
        name: "marketingBox",
        components:{referralDialog},
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
                        mobile_Not_logedIn: './images/mobile_Hebrew_Not_LogedIn.png',
                    },
                    english: {
                        logedIn: './images/mobile_English_LogedIn.png',
                        not_logedIn: './images/mobile_English_Not_LogedIn.png',
                    }
                }
            }
        },
        computed: {
            ...mapGetters(['accountUser', 'getMobMarketingState']),
            isIsrael() {
                return global.isIsrael;
            },
            isMobile() {
                return this.$vuetify.breakpoint.xsOnly
            },
            isLogedIn() {
                return this.accountUser;
            },
            imgSrc() {
                let imageSrc = '';
                let imagesSet = this.$vuetify.breakpoint.xsOnly ? this.mobile : this.desktop;
                imagesSet = this.isIsrael ? imagesSet.hebrew : imagesSet.english;
                imageSrc = this.isLogedIn ? imagesSet["logedIn"] : imagesSet["not_logedIn"];
                return imageSrc
            },
            userReferralLink(){
                return `${global.location.origin}` +"?referral=" + Base62.encode(this.accountUser.id) + "&promo=referral";
            },

        },
        methods: {
            getRelevanImage(){
                let imageSrc = '';
                let imagesSet = this.$vuetify.breakpoint.xsOnly ? this.mobile : this.desktop;
                imagesSet = this.isIsrael ? imagesSet.hebrew : imagesSet.english;
                imageSrc = this.isLogedIn ? imagesSet["logedIn"] : imagesSet["not_logedIn"];
                return imageSrc
            },
            relevantAction(){
            return  this.isLogedIn ?   this.updateRefState() : this.$router.push({path: '/register' + result, query: query});
            },
            updateRefState(){
                console.log('showing reffff')
                this.showReferral = !this.showReferral
            }

        },
    }
</script>

<style src="./marketingBox.less" lang="less">

</style>