<template>
    <div class="marketing-box-component">
        <div class="heading" v-if="$vuetify.breakpoint.smAndDown">
            <span class="heading-text" v-language:inner>marketingBox_title</span>
        </div>
        <v-card class="main-marketing-content transparent"  :class="imageClassABtest"
                :style="{ 'background-image': 'url(' + require(`${imgSrc}`) + ')' }" @click="promotionOpen()">
        </v-card>
    </div>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import Base62 from "base62"
    import analyticsService from '../../../services/analytics.service'

    export default {
        name: "marketingBox",
        data() {
            return {
                googleFormUrl: 'https://forms.gle/f5vDdwS3cDNNJUwA7'
            }
        },
        computed: {
            ...mapGetters(['accountUser']),
            isIsrael() {
                return global.lang.toLowerCase() === 'he'
            },
            isLogedIn() {
                return (this.accountUser != null)
            },
            imgSrc() {
                return this.isIsrael ? './images/heb_lessons.png' :  './images/eng_lessons.png'
            },
            imageClassABtest() {
                return this.imgSrc.replace('./images/', '');
            },
        },
        methods: {
            ...mapActions(['changemobileMarketingBoxState']),
            promotionOpen() {
                if (this.isLogedIn) {
                    analyticsService.sb_unitedEvent('MARKETING_BOX', 'OPEN_TUTOR');
                } else {
                    analyticsService.sb_unitedEvent('MARKETING_BOX', 'GO TO LOGIN');
                }
                return this.isLogedIn ? this.goToWebForm() : this.goToRegister();
            },
            goToRegister() {
                // this.changemobileMarketingBoxState();
                this.$router.push({name: 'registration'});
            },
            goToWebForm(){
                global.open(`${this.googleFormUrl}`, '_blank');
            }
        },
    }
</script>

<style src="./marketingBox.less" lang="less">

</style>