<template>
        <v-layout class="calendar-section mt-3">
            <v-flex xs12>
                <v-card class="elevation-0 caltab" v-if="isReady">
                    <calendar v-if="getShowCalendar"/>
                    <calendarEmptyState v-if="showEmptyState && !getShowCalendar"/>
                </v-card>
            </v-flex>
        </v-layout>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
import calendar from './calendar.vue'
import calendarEmptyState from './calendarEmptyState.vue'
import {LanguageService} from '../../services/language/languageService.js'
export default {
    components:{
        calendar,
        calendarEmptyState
    },
    data() {
        return {
            isReady: false,
            showCalendar: this.isCalendar
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
            return (this.isMyProfile && !this.getShowCalendar)
        },
    },
    methods: {
        ...mapActions(['updateCalendarStatus','updateToasterParams'])
    },
    created() {
        let self = this;
        this.$loadScript("https://apis.google.com/js/api.js").then(() => {
            self.updateCalendarStatus().then(()=>{
            },(err)=>{
                this.updateToasterParams({
                    toasterText: LanguageService.getValueByKey("put some error"),
                    showToaster: true,
                    toasterType: 'error-toaster'
                });
                console.error(err);
            }).finally(()=>{
                self.isReady = true
            })
        })
    },
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