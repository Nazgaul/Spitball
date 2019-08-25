<template>
        <v-layout class="calendar-section mt-3" >
            <v-flex xs12>
                <v-card class="elevation-0 caltab">
                    <calendar v-if="isCalendar || showCalendar"/>
                    <calendarEmptyState @updateCalendarStatus="update" v-if="showEmptyState"/>
                </v-card>
            </v-flex>
        </v-layout>
</template>

<script>
import { mapGetters } from 'vuex';
import calendar from './calendar.vue'
import calendarEmptyState from './calendarEmptyState.vue'
export default {
    components:{
        calendar,
        calendarEmptyState
    },
    data() {
        return {
            showCalendar: this.isCalendar
        }
    },
    computed: {
        ...mapGetters(['getProfile','accountUser']),
        isCalendar(){
            if(!!this.getProfile && this.getProfile.user){
                return this.getProfile.user.calendarShared;
            }
        },
        isMyProfile(){
            if(!!this.getProfile && !!this.accountUser){
                return (this.getProfile.user.id == this.accountUser.id)
            }
               return false;
        },
        showEmptyState(){
            return (this.isMyProfile && (!this.isCalendar && !this.showCalendar))
        }
    },
    methods: {
        update(){
            this.showCalendar = true;
        }
    }
}
</script>

<style lang="less">
    @import '../../styles/mixin.less';
    .calendar-section {
        @media (max-width: @screen-xs) {
            box-shadow: none;
            border-radius: 4px;
            
        }
        .caltab{
            padding: 22px;
            @media (max-width: @screen-xs) {
                padding: 10px;
                margin-bottom: 40px;
            }
        }
    }

</style>