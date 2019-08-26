<template>
    <div class="calendar-step-wrap" :class="[$vuetify.breakpoint.smAndUp ? 'px-0' : '']">
        <v-layout column wrap align-center justify-center class="calendar-step-wrap-cont">
            <img src="./images/calendar.png" alt="">
            <p v-language:inner="'becomeTutor_cal_step'"/>
            <v-btn  color="#4452FC"
                    round
                    :loading='isLoading'
                    class="white--text elevation-0 calbtnshare"
                    @click="shareCalendar()">
                <span v-language:inner="'becomeTutor_btn_cal_connect'"/>
            </v-btn>
        </v-layout>
    </div>
</template>

<script>
    import { mapActions} from 'vuex';
    export default {
        name: "calendarEmptyState",
        data() {
            return {
                isLoading: false
            }
        },
        methods: {
            ...mapActions(['gapiLoad','gapiSignIn',]),
            shareCalendar(){
                this.isLoading = true;
                let self = this
                this.gapiSignIn().then((res)=>{
                    this.$emit('updateCalendarStatus')
                },(err)=>{
                    this.isLoading = false;
                })
            },
        },
        created() {
            let self = this;
            this.$loadScript("https://apis.google.com/js/api.js").then(() => {
                setTimeout(() => {
                    self.gapiLoad();
                },);
            })
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
            overflow: hidden;
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
    }
</style>