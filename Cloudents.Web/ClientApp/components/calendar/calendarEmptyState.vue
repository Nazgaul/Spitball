<template>
    <div class="calendar-step-wrap" :class="[$vuetify.breakpoint.smAndUp ? 'px-0' : '']">
        <v-layout  column wrap align-center justify-center class="calendar-step-wrap-cont">
            <img v-if="!isSelectCalendar" src="./images/calendar.png" alt="">
            <p class="calendar-stepP" v-if="!isSelectCalendar" v-language:inner="'becomeTutor_cal_step'"/>
            <v-btn v-if="!isSelectCalendar" color="#4452FC"
                    round
                    :loading='isLoading'
                    class="white--text elevation-0 calbtnshare"
                    @click="shareCalendar">
                <span v-language:inner="'becomeTutor_btn_cal_connect'"/>
            </v-btn>
            <selectCalendarCMP v-if="isSelectCalendar"/>
            <v-btn v-if="isSelectCalendar" color="#4452FC"
                    round
                    :loading='isLoading'
                    class="white--text elevation-0 calbtnshare"
                    @click="initCalendar">
                <span v-language:inner="'becomeTutor_connect_mobile'"/>
            </v-btn>
        </v-layout>
    </div>
</template>
<script>
    import { mapActions, mapMutations} from 'vuex';
    import {LanguageService} from '../../services/language/languageService.js'
    import selectCalendarCMP from './calendarSelect.vue'

    export default {
        name: "calendarEmptyState",
        components:{selectCalendarCMP},
        data() {
            return {
                isLoading: false,
                isSelectCalendar: false
            }
        },
        methods: {
            ...mapActions(['gapiSignIn','updateToasterParams','updateSelectedCalendarList','getEvents']),
            shareCalendar(){
                this.isLoading = true;
                let self = this
                this.gapiSignIn().then((res)=>{
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
                    self.getEvents()
                })
            }
        },
        computed: {
            isMobile() {
                return this.$vuetify.breakpoint.smAndDown;
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
            .calendar-stepP{
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