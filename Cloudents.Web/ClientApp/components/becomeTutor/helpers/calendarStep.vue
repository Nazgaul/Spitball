<template>
    <div class="calendar-step-wrap" :class="[$vuetify.breakpoint.smAndUp ? 'px-0' : '']">
        <img v-if="(!isSelectCalendar && !isSelectHours) && !isMobile" class="cal-img" src="../images/calImgNew.png" alt="">

        <template v-if="!isSelectCalendar && !isSelectHours">
            <v-layout column wrap align-center justify-center class="calendar-step-wrap-cont">
                <img src="../images/calendar.png" alt="">
                <p v-language:inner="'becomeTutor_cal_step'"/>
                <v-btn  color="#4452FC"
                        round
                        :loading='isLoadingCalendar'
                        class="white-text elevation-0 calbtnshare"
                        @click="shareCalendar()">
                    <span v-language:inner="'becomeTutor_btn_cal_connect'"/>
                </v-btn>
            </v-layout>
        </template>

        <calendarSelect v-if="isSelectCalendar && !isSelectHours"/>
        <calendarHours v-if="!isSelectCalendar && isSelectHours"/>

        <template>
            <v-layout class="px-1 btns-cal-step"
                        :class="[$vuetify.breakpoint.smAndUp ? 'align-end justify-end' : 'align-center justify-center',isSelectCalendar || isSelectHours? 'resetMargin': '']">
                <v-btn @click="goToPreviousStep()" class="cancel-btn-step elevation-0" round outline flat>
                    <span v-language:inner="'becomeTutor_btn_back'"/>
                </v-btn>
                <v-btn  color="#4452FC"
                        :disabled="isBtn"
                        round
                        :loading='isLoading'
                        class="white-text elevation-0"
                        @click="btnDoneNextFunc">
                    <span v-language:inner="'becomeTutor_btn_next'"/>
                </v-btn>
            </v-layout>
        </template>
    </div>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    import { LanguageService } from "../../../services/language/languageService";
    import calendarSelect from '../../calendar/calendarSelect.vue'
    import calendarHours from '../../calendar/calendarHours.vue'
    export default {
        name: "calendarStep",
        data() {
            return {
                isLoading: false,
                isLoadingCalendar:false,
                isSelectCalendar: false,
                isSelectHours:false,
            }
        },
        components:{calendarSelect,calendarHours},
        computed: {
            ...mapGetters(['getCalendarAvailabilityIsValid']),
            isBtn(){
                if(this.isSelectHours){
                    if(this.getCalendarAvailabilityIsValid){
                        return false;
                    } else{
                        return true
                    }
                }else{
                  return false;  
                }
            },
            isMobile() {
                return this.$vuetify.breakpoint.smAndDown;
            }
        },
        methods: {
            ...mapActions(['gapiLoad','gapiSignIn']),
            goToPreviousStep() {
                this.$root.$emit('becomeTutorStep', 2);
            },
            shareCalendar(){
                this.isLoadingCalendar = true;
                let self = this
                this.gapiSignIn().then((res)=>{
                    self.isSelectCalendar = true
                },err=>{
                    this.isLoadingCalendar = false;
                })
            },
            btnDoneNextFunc(){
                if((!this.isSelectCalendar && !this.isSelectHours) || this.isSelectHours){
                    this.$root.$emit('becomeTutorStep', 4);
                    this.isLoading = false;
                    this.isLoadingCalendar = false;
                } else if(this.isSelectCalendar && !this.isSelectHours){
                    this.goSelectHour()
                }
            },
            goSelectHour(){
                this.isSelectCalendar = false;
                this.isSelectHours = true;
                this.isLoading = false;
                this.isLoadingCalendar = false;
            }
        },
        created() {
            let self = this;
            this.$loadScript("https://apis.google.com/js/api.js").then(() => {
                setTimeout(() => {
                    self.gapiLoad(['calendar']);
                },);
            })
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';

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
        position: relative;
        .cal-img{
            @media (max-width: @screen-xs) {
                bottom: 4px;
            }
            position: absolute;
            bottom: -14px;
            left: -24px;
            z-index: 0;
        }
        .calendar-step-wrap-cont{
            z-index: 1;
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
            //TODO: remove this because it caused an issue
            // p{
            //     margin: 0;
            //     font-size: 20px;
            //     line-height: 1.5;
            //     letter-spacing: -0.51px;
            //     color: @global-purple;
            //     padding: 0 150px;
            //     padding-bottom: 16px;
            //     text-align: center;
            //     @media (max-width: @screen-xs) {
            //         padding: 0 0 20px;

            //     }

            // }
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
        .blue-text {
            color: @global-blue;
        }
        .v-input__slot .v-text-field__slot label {
            color: @global-purple;
        }
        .btns-cal-step{
            &.resetMargin{
                margin-top: 0!important;
            }
            .cancel-btn-step{
                background: white !important;
                border: solid 1px @global-blue;
                color: @global-blue;
            }
            margin-top: 150px;
                @media (max-width: @screen-xs) {
                    // margin-top: 0;
                    // align-items: flex-end;
                }
                .v-btn{
                    @media (max-width: @screen-xs) {
                        text-transform: capitalize;
                    }
                }
        }
    }
</style>