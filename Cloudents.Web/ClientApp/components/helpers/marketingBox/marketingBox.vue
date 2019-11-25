<template>
    <div class="marketing-box-component">
        <div class="heading" v-if="$vuetify.breakpoint.smAndDown">
            <span class="heading-text" v-language:inner>marketingBox_title</span>
        </div>
        <v-card class="main-marketing-content transparent" @click="promotionOpen()">
            <img :src="imgBySiteType" alt="Private lessons">
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
            }
        },
        computed: {
            ...mapGetters(['accountUser', 'isFrymo']),
            isLogedIn() {
                return (this.accountUser != null)
            },
            imgBySiteType(){
                if(global.lang.toLowerCase() === 'he'){
                    return require('./images/Banner_Sept_he.jpg');
                }else{
                    return this.isFrymo ? require('./images/Frymo_Promotion.jpg') : require('./images/Banner_Sept_en.jpg');
                }
            }
        },
        methods: {
            ...mapActions(['updateRequestDialog', 'setTutorRequestAnalyticsOpenedFrom']),
            promotionOpen() {
                if (this.isLogedIn) {
                    analyticsService.sb_unitedEvent('MARKETING_BOX', 'REGISTERED OPEN_TUTOR');
                } else {
                    analyticsService.sb_unitedEvent('MARKETING_BOX', 'NOT REGISTERED OPEN_TUTOR');
                }
                return this.isLogedIn ? this.goToWebForm() : this.goToRegister();
                // return this.isLogedIn ? this.goToWebForm() : this.goToRegister();
            },
            goToRegister() {
                // this.$router.push({name: 'registration'});
                this.setTutorRequestAnalyticsOpenedFrom({
                    component: 'marketingBox',
                    path: this.$route.path
                });
                this.updateRequestDialog(true);
            },
            goToWebForm(){
                this.setTutorRequestAnalyticsOpenedFrom({
                    component: 'marketingBox',
                    path: this.$route.path
                });
                this.updateRequestDialog(true);
            }
        },
    }
</script>

<style src="./marketingBox.less" lang="less">

</style>