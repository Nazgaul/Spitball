<template>
    <div class="marketing-box-component ">
        <div class="heading" v-if="$vuetify.breakpoint.smAndDown">
            <span class="heading-text" v-language:inner>marketingBox_title</span>
        </div>
        <v-card class="main-marketing-content transparent"
                @click="promotionOpen()">
            <img v-if="isIsrael" src="./images/Banner_Sept_he.jpg" alt="Private lessons">
            <img v-else src="./images/Banner_Sept_en.jpg" alt="Private lessons">
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
            ...mapGetters(['accountUser']),
            isIsrael() {
                return global.lang.toLowerCase() === 'he'
            },
            isLogedIn() {
                return (this.accountUser != null)
            },
        },
        methods: {
            ...mapActions(['changemobileMarketingBoxState', 'updateRequestDialog', 'setTutorRequestAnalyticsOpenedFrom']),
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