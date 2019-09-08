<template>
    <div class="calendar-step-wrap" :class="[$vuetify.breakpoint.smAndUp ? 'px-0' : '']">
        <img v-if="!isSelectCalendar && !isMobile" class="cal-img" src="../images/calImgNew.png" alt="">

        <v-layout v-if="!isSelectCalendar" column wrap align-center justify-center class="calendar-step-wrap-cont">
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

        <calendarSelect v-if="isSelectCalendar"/>

        <v-layout class="px-1 btns-cal-step"
                  :class="[$vuetify.breakpoint.smAndUp ? 'align-end justify-end' : 'align-center justify-center',isSelectCalendar? 'resetMargin': '']">
            <v-btn @click="goToPreviousStep()" class="cancel-btn-step elevation-0" round outline flat>
                <span v-language:inner="'becomeTutor_btn_back'"/>
            </v-btn>
            <v-btn  color="#4452FC"
                    round
                    :loading='isLoading'
                    class="white-text elevation-0"
                    @click="submit()">
                <span v-language:inner="'becomeTutor_btn_done'"/>
            </v-btn>

        </v-layout>
    </div>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    import { LanguageService } from "../../../services/language/languageService";
    import calendarSelect from '../../calendar/calendarSelect.vue'
    export default {
        name: "calendarStep",
        data() {
            return {
                isLoading: false,
                isLoadingCalendar:false,
                isSelectCalendar: false,
            }
        },
        components:{calendarSelect},
        computed: {
            isMobile() {
                return this.$vuetify.breakpoint.smAndDown;
            },
        },
        methods: {
            ...mapActions(['updateTutorDialog',
                            'gapiLoad',
                            'gapiSignIn',
                            'sendBecomeTutorData',
                            'updateAccountUserToTutor',
                            'updateToasterParams',
                            'updateTeachingClasses',
                            ]),
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
            submit(){
                let self = this
                this.isLoadingCalendar = false;
                this.isLoading = true;
                this.sendBecomeTutorData().then(
                    (resp) => {
                        self.$root.$emit('becomeTutorStep', 4);
                        self.updateAccountUserToTutor(true);
                        self.updateToasterParams({
                            toasterText: LanguageService.getValueByKey("becomeTutor_already_submitted"),
                            showToaster: true,
                            toasterTimeout: 5000
                        });
                        self.updateTeachingClasses();
                    },(error) => {
                        let isConflict = error.response.status === 409;
                        if(isConflict) {
                            self.updateToasterParams({
                                toasterText: LanguageService.getValueByKey("becomeTutor_already_submitted"),
                                showToaster: true,
                                toasterTimeout: 5000
                            });
                            self.updateTutorDialog(false);
                        }
                    }).finally(() => {                      
                        self.isLoading = false;
                });
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
            p{
                margin: 0;
                font-size: 20px;
                line-height: 1.5;
                letter-spacing: -0.51px;
                color: @global-purple;
                padding: 0 150px;
                padding-bottom: 16px;
                text-align: center;
                @media (max-width: @screen-xs) {
                    padding: 0 0 20px;

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