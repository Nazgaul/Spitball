<template>
        <tr>
            <td class="tdDayName"><p class="pDayName" v-html="dayName"/></td>

            <td class="tdDayHourSelect">
                <v-select :class="['select-cal',{'dayOff':isDayOff}]"
                        v-model="selectedHourFrom" 
                        height="30" dense
                        :append-icon="'sbf-arrow-fill'" 
                        :items="hoursList" outline>
                    <template slot="selection" slot-scope="data">{{timeFormat(data.item)}}</template>
                    <template slot="item" slot-scope="item">{{timeFormat(item.item)}}</template>
                </v-select>

                <span v-if="!isDayOff" class="dividers" v-language:inner="'calendar_to'"/>

                <v-select v-if="!isDayOff" class="select-cal" height="30" dense tag="span"
                            v-model="selectedHourTo"
                            :append-icon="'sbf-arrow-fill'" 
                            :items="hoursToList" outline>
                    <template slot="selection" slot-scope="data">{{timeFormat(data.item)}}</template>
                    <template slot="item" slot-scope="item">{{timeFormat(item.item)}}</template>
                </v-select>
                
                <span v-if="!isDayOff && (selectedHourTo < 23 && !isAddTimeSlot)"
                      @click="isAddTimeSlot = true" class="addTime"
                      v-language:inner="isMobile?'calendar_add_time_mobile':'calendar_add_time'"/>

                    <span class="dividersAnd" 
                           v-language:inner="'calendar_and'"
                           v-show="isMobile &&(!isDayOff && isAddTimeSlot)"/>

                    <span class="dividers" v-language:inner="'calendar_and'" 
                          v-show="!isMobile &&(!isDayOff && isAddTimeSlot)"/>
                <div :style="{'display':isMobile?'':'inline-block'}" :class="!showAdditional || isDayOff?'additionalHoursDisplay':''" > 
                        <v-select class="select-cal" height="30" dense
                                v-model="selectedAdditionalHourFrom"
                                :append-icon="'sbf-arrow-fill'" 
                                :items="hoursAdditionaFromList" outline>
                            <template slot="selection" slot-scope="data">{{timeFormat(data.item)}}</template>
                            <template slot="item" slot-scope="item">{{timeFormat(item.item)}}</template>
                        </v-select>

                        <span class="dividers" v-language:inner="'calendar_to'"/>

                        <v-select :class="['select-cal',{'mt-2':isMobile}]" height="30" dense
                                v-model="selectedAdditionalHourTo"
                                :append-icon="'sbf-arrow-fill'" 
                                :items="hoursAdditionaToList" outline>
                            <template slot="selection" slot-scope="data">{{timeFormat(data.item)}}</template>
                            <template slot="item" slot-scope="item">{{timeFormat(item.item)}}</template>
                        </v-select>
                    
                        <removeTimeSVG @click="closeAdditionalTime" class="removeTime"/> 
                </div>
            </td>
        </tr>
</template>

<script>
import { mapActions, mapGetters } from "vuex";
import removeTimeSVG from './images/removeTime.svg';
import { LanguageService } from "../../services/language/languageService";

export default {
    components:{removeTimeSVG},
    props:{
        day:{
            type: Number,
            required: true
        },
        dayIndex:{}
    },
    data() {
        return {
            isAddTimeSlot: false,
            hoursList:[`${LanguageService.getValueByKey("calendar_day_off")}`,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23],
            selectedHourTo:21,
            selectedAdditionalHourFrom:'',
            selectedAdditionalHourTo:'',
            availabilityDay: {
                day:this.day,
                timeFrames:[]
            },
            selectedHourFrom:9,
            fullHour:1,
        }
    },
    computed: {
        ...mapGetters(['getIsCalendarShared','getIntervalFirst','getCalendarAvailabilityState','accountUser','getProfile']),
        availabilityDayState(){
            if(this.getCalendarAvailabilityState !== null){
                let calendarDayList = []
                this.getCalendarAvailabilityState.forEach(calendarDay=>{
                    if(calendarDay.day == this.day){
                        calendarDayList.push(calendarDay)
                    }
                });
                return calendarDayList.length? calendarDayList: null;
            }else{
                return null;
            }
        },
        dayName(){
            let options = { weekday: this.isMobile? 'short':'long' }
            let dayDate = new Date(`2017-01-0${this.day+1}`)
            return dayDate.toLocaleString(`${global.lang}-${global.country}`,options )
        },
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        isDayOff(){
            return (this.selectedHourFrom == this.hoursList[0])
        },
        hoursToList(){
            let start = this.selectedHourFrom +this.fullHour;
            // if(this.availabilityDayState !== null && this.availabilityDayState !== undefined){
            //     start = +this.availabilityDayState[0].from.split(':')[0]+this.fullHour;
            // }else{
                // start = this.selectedHourFrom +this.fullHour;
            // }
            let end = 24;    
            let difference = Math.abs(start-end);
            let rangeArray = new Array(difference + 1).fill(undefined).map((val, key) => {
                return start > end ? start - key : start + key;
            })
            if(this.selectedHourTo <= rangeArray[0]){
                this.setSelectedHourTo(rangeArray[0])
            }
            return rangeArray
        },

        hoursAdditionaFromList(){
            let listTo = this.hoursList.filter(hour=> hour > this.selectedHourTo)
            if(this.selectedAdditionalHourFrom <= listTo[0]){
                this.setSelectedAdditionalHourFrom(listTo[0])
            }
            return listTo
        },
        hoursAdditionaToList(){
            let listTo = this.hoursAdditionaFromList.filter(hour=> hour > this.selectedAdditionalHourFrom)
                listTo.push(24)
            if(this.selectedAdditionalHourTo <= listTo[0]){
                this.setSelectedAdditionalHourTo(listTo[0])
            }
            return listTo
            
        },
        showAdditional(){
            return ( this.isAddTimeSlot && this.selectedHourTo < 24)
        }, 
    },
    methods: {
        ...mapActions(['updateStateAvailabilityCalendar']),
        setSelectedHourTo(val){
            this.selectedHourTo = val
        },
        setSelectedAdditionalHourFrom(val){
            this.selectedAdditionalHourFrom = val;
        },
        setSelectedAdditionalHourTo(val){
            this.selectedAdditionalHourTo = val;
        },
        timeFormat(time){
            if(time == this.hoursList[0]) {
                return time
            }else{
                return (time < 10) ? `0${time}:00` : `${time}:00`;
            }
        },
        timeFormatISO(time){
            let currentDate = new Date()
            return new Date(currentDate.getFullYear(),currentDate.getMonth() , currentDate.getDay(), time,0,0,0).toISOString().slice(11,19)
        },
        closeAdditionalTime(){
            this.isAddTimeSlot = false;
            this.selectedAdditionalHourFrom = ''
            this.selectedAdditionalHourTo = ''
        },
        updateAvailability(){
            let timeFrames = [   
                this.timeFormatISO(this.selectedHourFrom),
                this.timeFormatISO(this.selectedHourTo)
                ]
            if(this.isAddTimeSlot){
                let additionalHourTimeFrames = [
                    this.timeFormatISO(this.selectedAdditionalHourFrom),
                    this.timeFormatISO(this.selectedAdditionalHourTo)
                ]
                timeFrames = [...timeFrames,...additionalHourTimeFrames]
            }
            
            this.availabilityDay.timeFrames = [...timeFrames]
        },
        runUpdate(){
            if(this.selectedHourFrom == this.hoursList[0]){
                this.availabilityDay.timeFrames = [] 
                return}
            if((!this.selectedAdditionalHourFrom && this.isAddTimeSlot) && this.selectedHourTo >= 23){return}
            if(!this.selectedAdditionalHourTo && this.isAddTimeSlot){
                this.updateAvailability()
                return}

            this.updateAvailability()
        },
        initialHoursList(){
            if(this.availabilityDayState !== null){
                let start = 8;
                let end = 24;    
                let difference = Math.abs(start-end);
                let rangeArray = new Array(difference + 1).fill(undefined).map((val, key) => {
                    return start > end ? start - key : start + key;
                })
                rangeArray.unshift(LanguageService.getValueByKey("calendar_day_off"))
                this.hoursList = rangeArray
                this.selectedHourFrom = +this.availabilityDayState[0].from.split(':')[0];
                this.selectedHourTo = +this.availabilityDayState[0].to.split(':')[0]; 
            }else{
                this.selectedHourFrom = LanguageService.getValueByKey("calendar_day_off")
            }
        },
        initialAdditionalHoursList(){
            let from = +this.availabilityDayState[1].from.split(':')[0]
            let to = +this.availabilityDayState[1].to.split(':')[0]
            if(to === 0){
                to = 24;
            }
            this.setSelectedAdditionalHourFrom(from)
            this.setSelectedAdditionalHourTo(to)
            this.isAddTimeSlot = true;
        },
    },
    watch: {
        isAddTimeSlot:function(val){
            if(!val){
                this.closeAdditionalTime()
            }
        },
        selectedHourTo:function(val) {
            if(this.isAddTimeSlot && val >= 23){
                this.closeAdditionalTime()
            }
        },

    },
    created() {
        this.selectedHourFrom = this.getIntervalFirst;
    },
    mounted() {
        this.runUpdate()
        this.updateStateAvailabilityCalendar(this.availabilityDay)
            if(this.getIsCalendarShared === true){
                this.initialHoursList()
                if(this.availabilityDayState !== null && this.availabilityDayState.length > 1){
                    this.initialAdditionalHoursList()
                }
            }
    },
    updated() {
        this.runUpdate()
        this.updateStateAvailabilityCalendar(this.availabilityDay) 
    },
    
}
</script>

<style lang="less">
    @import '../../styles/mixin.less';
    // table{
        // tr td:first-child{
        //     padding-right: 6px;
        //     white-space:nowrap;
        //     vertical-align: baseline;
        //     padding-top: 4px;
        // }
        td{
            &.tdDayName{
                .pDayName{
                    margin: 0;
                    padding: 0;
                    font-size: 14px;
                    font-weight: 600;  
                    text-align: inherit;
                }
            }
            &.tdDayHourSelect{
                span{
                    font-size: 14px;
                    &.addTime{
                        padding-left: 10px;
                        cursor: pointer;
                        font-weight: 600;
                        color: @global-blue;
                        @media (max-width: 374px) {
                            padding-left: 0;
                        }
     
                    }
                }
                .additionalHoursDisplay{
                    // display:inline-block;
                    visibility: hidden;
                    @media (max-width: @screen-xs) {
                        display: none;
                    }
                }
                .dividers{
                    @media (max-width: 374px) {
                       padding: 0 8px; 
                    }
                    padding: 0 10px;
                }
                .dividersAnd{
                        margin-bottom: 26px;
                        padding-left: 10px;        
                }
                .select-cal{
                    display: inline-block;
                    max-width: 80px;
                    min-width: 80px;
                }
                .dayOff{
                    max-width: 202px;
                }
                .removeTime{
                    cursor: pointer;
                    margin-left: 10px;
                    vertical-align: middle;
                }
                .v-input{
                    .v-input__control{
                        .v-input__slot{
                            margin: 0;
                            .v-select__slot {
                                .v-input__append-inner{
                                    margin-top: 4px !important;
                                    .v-input__icon{
                                        i{
                                            font-size: 6px !important;
                                            color: #26262f;
                                        }
                                    }
                                }
                            }
                        }
                        .v-text-field__details{
                            display: none;
                        }
                    }
                }
        
                .v-select.v-text-field--enclosed:not(.v-text-field--single-line) .v-select__selections {
                    padding-top: 0 !important;
                }
                .v-text-field--box .v-input__slot,
                .v-text-field--outline .v-input__slot {
                    min-height: auto!important;
                    display: flex!important;
                    align-items: center!important;
                }
                .v-text-field.v-text-field--enclosed>.v-input__control>.v-input__slot{
                    padding: 0 0 0 8px;
                    font-size: 14px;
                    color: @global-purple;
                }
                .theme--light.v-text-field--outline>.v-input__control>.v-input__slot{
                    border-radius: 6px!important;
                    border: solid 1px #b8c0d1!important;
                }
                .v-select__selection {
                    max-width: initial;
                }
                .v-select__selection--comma{
                    .giveMeEllipsisOne();
                }
            }
            }
    // }

</style>