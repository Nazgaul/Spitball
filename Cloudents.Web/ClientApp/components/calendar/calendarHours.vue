<template>
    <div class="calendarHours">
        <div class="calendarHoursCont">
            <div class="calendarHoursTitle">
                <h1 v-language:inner="'calendar_calendar_hour_title'"/>
                <h2 v-language:inner="'calendar_calendar_hour_subtitle'"/>
            </div>
    <table>
        <tbody>
            <calendarDay v-for="day in days" :day="day" :key="day" @change.prevent="test()"/>
        </tbody>
    </table>
        </div>
    </div>
</template>

<script>
import calendarDay from './calendarDay.vue';
export default {
    components:{calendarDay},
    data() {
        return {
            days:[0,1,2,3,4,5,6]   
        }
    }
}
</script>

<style lang="less">
    @import '../../styles/mixin.less';
    .calendarHours{
        table {
        width:100%; 
        border-collapse: separate;  
        border-spacing: 0 10px;
            @media (max-width: @screen-xs) {
                width: 300px;
                margin: 0 auto;
            }
            @media (max-width: 374px) {
                width: 100%;
            }
            tr td:first-child{
            padding-right: 6px;
            white-space:nowrap;
            vertical-align: baseline;
            padding-top: 4px;
            }
        }
        color: @global-purple;
        display: flex;
        flex-direction: column;
        align-items: center;
        .calendarHoursTitle{
            margin-bottom: 16px;
            h1{
                font-size: 16px;
                font-weight: 600;
                font-style: normal;
                font-stretch: normal;
                line-height: 1.38;
                letter-spacing: normal;
            }
            h2{
                font-size: 14px;
                font-weight: normal;
                font-style: normal;
                font-stretch: normal;
                line-height: normal;
                letter-spacing: normal;
            }
        }
    }
</style>
