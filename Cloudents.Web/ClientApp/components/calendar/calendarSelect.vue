<template>
    <div class="calendarSelect">
        <p class="calendarSelectP"  v-t="isMobile?'becomeTutor_select_calendar_title_mobile':'becomeTutor_select_calendar_title'"/>   
        <div class="calendarLines" :style="paddingScroll">
            <div v-for="calendar in calendarsList" :key="calendar.id">
                <div class="singleCalendarLine">
                    <div class="calendarLine">
                        <calendarSVG class="svg-calendar"/>
                        <span>{{calendar.name}}</span>
                    </div>
                    <v-switch multiple :value="calendar" v-model="selectedCalendars" :ripple="false" color="#4452fc"/>
                </div>
                <v-divider class="divider-calendar"/>
            </div>
        </div>
    </div>
</template>

<script>
import calendarSVG from './images/calendarSVG.svg'
import { mapActions, mapGetters } from 'vuex';
export default {
    components:{calendarSVG},
    data() {
        return {
            selectedCalendars: []
        }
    },
    computed: {
        ...mapGetters(['getCalendarsList']),
        calendarsList(){
            return this.getCalendarsList
        },
        isMobile() {
            return this.$vuetify.breakpoint.smAndDown;
        },
        paddingScroll(){
            if(!this.isMobile && this.calendarsList.length > 5){
                return `padding-right: 10px`
            } else{
                return '';
            }
        }
    },
    methods: {
        ...mapActions(['updateStateSelectedCalendarList'])
    },
    watch: {
        selectedCalendars:function(val){
            this.updateStateSelectedCalendarList(val)
        }
    },
    created(){
        if(this.calendarsList){
            this.calendarsList.forEach((calendar)=>{
                if(calendar.isShared){
                    this.selectedCalendars.push(calendar)
                } 
            })
        }
    }
}
</script>

<style lang="less">
    @import '../../styles/mixin.less';
                ::-webkit-scrollbar-track {
                background: #f5f5f5; 
            }
            ::-webkit-scrollbar {
                width: 6px;
            }
            ::-webkit-scrollbar-thumb {
                background: #b5b8d9 !important;
                border-radius: 4px !important;
            }
    .calendarSelect{
        width: fit-content;
        margin: auto;
        // margin-top: 12px !important;
        @media (max-width: @screen-xs) {
            width: 100%;
            P{
                width: initial;
            }
        }
        .calendarSelectP{
            margin: 0!important;
            padding: 0 !important;
            font-size: 16px!important;
            line-height: 1.56!important;
            text-align: center!important;
            color: @global-purple;
            margin-bottom: 36px !important;
        }
        .calendarLines{
            min-height: 256px;
            max-height: 256px;
            overflow-x: hidden;
            max-width: 410px;
            margin: auto;
            @media (max-width: @screen-xs) {
                min-height: inherit;
                max-height: inherit;
                overflow-x: inherit;
            }
            margin-bottom: 20px;
            .divider-calendar{
                margin-bottom: 12px;
                border-color: #dddddd !important;
            }
            .singleCalendarLine{
                display: flex;
                justify-content: space-between;
                align-items: center;
                .calendarLine{
                    margin-left: 8px;
                    width: calc(~"100% - 60px");
                    margin-bottom: 14px;
                    .svg-calendar{
                        margin-right: 8px;
                        font-size: 21px;
                        vertical-align: middle;
                    }
                    span{
                        .giveMeEllipsisOne();
                        font-size: 14px;
                        line-height: 2.14;
                        letter-spacing: -0.36px;
                        color: @global-purple;
                        line-height: normal;
                        display: inline-block;
                        width: calc(~"100% - 32px");
                        vertical-align: middle;
                    }
                }
                .theme--light.v-input--switch__thumb{
                    color: white
                }
                .v-input--selection-controls{
                    margin: 0;
                    flex-grow: 0;
                    flex-shrink: 0;

                    .v-input__slot {
                        margin-bottom: 14px;
                        }
                    .v-messages{
                        display: none;
                    }
                }
            }
        }
    }
</style>
