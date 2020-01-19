<template>
    <div class="calendar-step-wrap" :class="[$vuetify.breakpoint.smAndUp ? 'px-0' : '']">
        <v-layout  column wrap align-center justify-center class="calendar-step-wrap-cont">
            <template v-if="!isSelectCalendar && !isSelectHours">
                <img src="./images/calendar.png" alt="">
                <p class="pEmptyCalendar" v-language:inner="'becomeTutor_cal_step'"/>
            </template>

            <selectCalendarCMP v-if="isSelectCalendar  && !isSelectHours"/>

            <calendarHoursCMP v-if="!isSelectCalendar && isSelectHours" class="my-3"/>


            <v-btn color="#4452FC" rounded :loading='isLoading'
                    class="white--text elevation-0 calbtnshare"
                    @click="emptyStateFunctions">
                <span v-language:inner="emptyStateResources"/>
            </v-btn>
        </v-layout>
    </div>
</template>
<script>
    import { mapActions} from 'vuex';
    import {LanguageService} from '../../services/language/languageService.js'
    import selectCalendarCMP from './calendarSelect.vue';
    import calendarHoursCMP from './calendarHours.vue'

    export default {
        name: "calendarEmptyState",
        components:{selectCalendarCMP,calendarHoursCMP},
        data() {
            return {
                isLoading: false,
                isSelectCalendar: false,
                isSelectHours:false
            }
        },
        computed: {
            isMobile() {
                return this.$vuetify.breakpoint.smAndDown;
            },
            emptyStateResources(){
                if(!this.isSelectCalendar && !this.isSelectHours){
                    return `becomeTutor_btn_cal_connect`
                }
                if(this.isSelectCalendar && !this.isSelectHours){
                    return `becomeTutor_btn_next`
                }
                if(!this.isSelectCalendar && this.isSelectHours){
                    return `becomeTutor_connect_mobile`
                }
                return '';
            }
        },
        methods: {
            ...mapActions(['gapiSignIn','updateToasterParams','updateSelectedCalendarList','getEvents','updateAvailabilityCalendar']),
            shareCalendar(){
                this.isLoading = true;
                let self = this
                this.gapiSignIn().then(()=>{
                    self.isSelectCalendar = true
                    this.isLoading = false;
                    
                },(err)=>{
                    this.isLoading = false;
                    if(err.error) return 
                    this.updateToasterParams({
                        toasterText: LanguageService.getValueByKey("tutorRequest_request_error"),
                        showToaster: true,
                        toasterType: "error-toaster"
                    });
                })
            },
            initCalendar(){ 
                let self = this;
                this.isLoading = true;
                this.updateSelectedCalendarList().then(()=>{
                    self.updateAvailabilityCalendar().then(()=>{
                        self.getEvents()
                    })
                })
            },
            goSelectHour(){
                this.isLoading = false;
                this.isSelectCalendar = false;
                this.isSelectHours = true;
            },
            emptyStateFunctions(){
                if(!this.isSelectCalendar && !this.isSelectHours){
                    this.shareCalendar()
                    return
                }
                if(this.isSelectCalendar && !this.isSelectHours){
                    this.goSelectHour()
                    return
                }
                if(!this.isSelectCalendar && this.isSelectHours){
                    if(this.$route.name === 'myCalendar'){
                        this.$emit('updateCalendar')
                        return
                    }else{
                        this.initCalendar()
                        return
                    }
                }
            },
        },
    };
</script>

<style lang="less">
    @import '../../styles/mixin.less';

    .calendar-step-wrap {
                @media (max-width: @screen-xs) {
            display: flex;
            flex-direction: column;
            align-items: center;
            padding: 0 !important;
            overflow: visible;
            justify-content: space-between;
            height: inherit
        }
        .calendar-step-wrap-cont{
                @media (max-width: @screen-xs) {
                    display: flex;
                    justify-content: start;
                }

            img{
                padding-bottom: 22px;
                @media (max-width: @screen-xs) {
                    padding-top: 16px;
                    padding-bottom: 32px;
                }
            }
            .pEmptyCalendar{
                margin: 0;
                font-size: 16px;
                line-height: 1.5;
                letter-spacing: -0.51px;
                color: @global-purple;
                padding: 0 150px;
                padding-bottom: 16px;
                text-align: center;
                @media (max-width: @screen-xs) {
                    padding: 0 0 20px;
                    font-size: 16px;
                    width: 100%;
                }
            }


            .v-btn{
                @media (max-width: @screen-xs) {
                  height: 46px;
              }  
            .v-btn__content{
                span{
                    letter-spacing: -0.42px;
                    text-transform: none;
                    font-size: 16px;
                }
            }
            }
        }
    }
</style>