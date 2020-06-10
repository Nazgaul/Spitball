<template>
    <v-container @click="openRequestTutor()" class="suggestCard-container" >
        <v-layout align-center xs12 sm6 wrap class="suggestCard-sections">
            <div class="suggestCard-texts">
                <h3>{{suggestText}}</h3>
                <h4 v-t="'suggestCard_body'"></h4>
            </div>
            <div class="suggestCard-btn-section">
                <button class="suggestCard-btn" v-t="'suggestCard_btn'"></button>
            </div>
        </v-layout>
    </v-container>
</template>
<script>
import analyticsService from '../../services/analytics.service';
import { mapActions } from 'vuex';
export default {
    computed: {
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        suggestText() {
            if(this.isMobile) {
                return this.$t('suggestCard_title_mobile')
            }
            return this.$t('suggestCard_title')
        }
    },
    methods: {
        ...mapActions(['setTutorRequestAnalyticsOpenedFrom','updateRequestDialog']),
        openRequestTutor() {
            analyticsService.sb_unitedEvent('Tutor_Engagement', 'request_box');
            this.setTutorRequestAnalyticsOpenedFrom({
                component: 'suggestCard',
                path: this.$route.path
            });
            this.updateRequestDialog(true);
        },
    },
}
</script>
<style src="./suggestCard.less" lang="less"></style>