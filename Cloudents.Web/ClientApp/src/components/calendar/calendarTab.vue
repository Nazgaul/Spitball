<template>
    <v-layout class="calendar-section" v-if="showCalendarTab">
        <v-flex xs12 class="">
            <v-progress-circular class="progress-calendar" v-show="!isReady && !studentEmptyState" indeterminate :size="150" width="3" color="info"/>
            <v-card class="caltab" v-if="isReady">
                <calendar v-if="showCalendar"/>
            </v-card>
            <v-card class="caltab" v-show="studentEmptyState">
                <span v-t="'calendar_empty_state_student'"></span>
            </v-card>
        </v-flex>
    </v-layout>
</template>

<script>
const calendar = () => import('./calendar.vue');

export default {
    props: {
        showCalendarTab: {}
    },
    components:{
        calendar,
    },
    data() {
        return {
            isReady: false,
            studentEmptyState: false,
        }
    },
    computed: {
        isMyProfile(){
            return this.$store.getters.getIsMyProfile
        },
        showCalendar() {
            return this.$store.getters.getShowCalendar
        },
        showEmptyState(){
            return (this.isMyProfile && !this.showCalendar)
        },
    },
    beforeDestroy() {
        this.$store.dispatch('resetCalendar')
    },
    created() {
        let self = this;
        //this.$loadScript("https://apis.google.com/js/api.js").then(() => {
        this.$store.dispatch('updateCalendarStatus').then(()=>{
            self.isReady = true
        },()=>{
            if(!self.isMyProfile){
                self.studentEmptyState = true;
            }
        })
        //})
    },
}
</script>

<style lang="less">
@import '../../styles/mixin.less';
.calendar-section {
    position: relative;
    max-width: 960px;
    border-radius: 8px !important;
    @media (max-width: @screen-xs) {
        box-shadow: none;
        border-radius: 4px;
    }
    .close-btn {
        cursor: pointer;
        position: absolute;
        font-size: 12px !important;
        z-index: 6;
        right: 0;
        padding-right: 16px;
        padding-top: 16px;
    }
    .progress-calendar {
        position: absolute;
        z-index: 5;
        top: 38%;
        left: 38%;
    }
    .caltab{
        // ask shiran for box shadow
        box-shadow: none;
        // box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
        padding: 40px 22px;
        border-radius: 8px;
        @media (max-width: @screen-xs) {
                box-shadow: none;
            padding: 10px;
            // margin-bottom: 40px;
        }
    }
}

</style>