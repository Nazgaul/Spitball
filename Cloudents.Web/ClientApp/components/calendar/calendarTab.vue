<template>
    <v-layout class="calendar-section mt-4">
        <v-icon v-if="!!globalFunctions" @click="globalFunctions.closeCalendar()" class="close-btn">sbf-close</v-icon>
        <v-flex xs12>
            <v-progress-circular class="progress-calendar" v-show="!isReady && !studentEmptyState" indeterminate :size="150" width="3" color="info"/>
            <v-card class="elevation-0 caltab" v-if="isReady">
                <calendar v-if="getShowCalendar"/>
                <calendarEmptyState v-if="showEmptyState && !getShowCalendar"/>
            </v-card>
            <v-card class="elevation-0 caltab safdsfsfd" v-show="studentEmptyState">
                <span v-language:inner="'calendar_empty_state_student'"></span>
            </v-card>
        </v-flex>
    </v-layout>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
import calendar from './calendar.vue'
import calendarEmptyState from './calendarEmptyState.vue'
//import {LanguageService} from '../../services/language/languageService.js'
export default {
    components:{
        calendar,
        calendarEmptyState
    },
    props:{
        globalFunctions:{
            type: Object,
        }
    },
    data() {
        return {
            isReady: false,
            showCalendar: this.isCalendar,
            studentEmptyState: false,
            isDashboard: this.$route.name == 'myCalendar'
        }
    },
    computed: {
        ...mapGetters(['getProfile','accountUser','getShowCalendar']),
        isMyProfile(){
            if(!!this.getProfile && !!this.accountUser){
                return (this.getProfile.user.id == this.accountUser.id)
            }
               return false;
        },
        showEmptyState(){
            return ((this.isMyProfile || this.isDashboard) && !this.getShowCalendar)
        },
    },
    methods: {
        ...mapActions(['updateCalendarStatus','updateToasterParams'])
    },
    created() {
        let self = this;
        this.$loadScript("https://apis.google.com/js/api.js").then(() => {
            self.updateCalendarStatus().then(()=>{
                self.isReady = true
            },()=>{
                if(!self.isMyProfile){
                    self.studentEmptyState = true;
                }
            })
        })
    },
}
</script>

<style lang="less">
    @import '../../styles/mixin.less';
    .calendar-section {
        
        .close-btn{
            cursor: pointer;
            position: absolute;
            font-size: 12px !important;
            z-index: 6;
            right: 0;
            padding-right: 16px;
            padding-top: 16px;
        }

        @media (max-width: @screen-xs) {
            box-shadow: none;
            border-radius: 4px;
            
        }
          position: relative;
  .progress-calendar{
    position: absolute;
    z-index: 5;
    top: 38%;
    left: 38%;
  }
        .caltab{
            padding: 40px 22px;
            @media (max-width: @screen-xs) {
                padding: 10px;
                margin-bottom: 40px;
            }
        }
    }

</style>