<template>
    <div class="calendarSelect">
        <p v-language:inner="isMobile?'becomeTutor_select_calendar_title_mobile':'becomeTutor_select_calendar_title'"/>
        <div class="calendarLines" :style="paddingScroll">
            <div v-for="calendar in calendarsList" :key="calendar.id">
                <div class="singleCalendarLine">
                    <div class="calendarLine">
                        <calendarSVG class="svg-calendar"/>
                        <span>{{calendar.name}}</span>
                    </div>
                    <v-switch multiple v-model="selectedCalendars" :value="calendar" :ripple="false" color="#4452fc"/>
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
        margin-top: 12px !important;
        @media (max-width: @screen-xs) {
            width: 100%;
            P{
                width: initial;
            }
        }
        p{
            white-space: pre-line;
            margin: 0;
            padding: 0;
            font-size: 16px;
            line-height: 1.56;
            text-align: center;
            color: @global-purple;
            margin-bottom: 36px;
            width: fit-content;
        }
        .calendarLines{
            min-height: 256px;
            max-height: 256px;
            overflow-x: hidden;
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
                .calendarLine{
                    margin-left: 8px;
                    .svg-calendar{
                        margin-right: 8px;
                        font-size: 21px;
                        vertical-align: sub;
                    }
                    span{
                        font-size: 14px;
                        line-height: 2.14;
                        letter-spacing: -0.36px;
                        color: @global-purple;
                    }
                }
                .v-input--switch__thumb{
                    color: white !important;
                }
                .v-input--selection-controls{
                    margin: 0;
                    padding-top: 2px;
                    max-width: fit-content;
                    margin-right: -4px;
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
