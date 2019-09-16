<template>
        <tr>
            <td class="tdDayName">
                <p class="pDayName" v-html="dayName"/>
            </td>
            

            <td class="tdDayHourSelect">
                    <v-select :class="['select-cal',{'dayOff':isDayOff}]"
                            v-model="selectedHourFrom" 
                            height="30" dense
                            :append-icon="'sbf-arrow-down'" 
                            :items="hoursList" outline>
                        <template slot="selection" slot-scope="data">{{timeFormat(data.item)}}</template>
                        <template slot="item" slot-scope="item">{{timeFormat(item.item)}}</template>
                    </v-select>

                    <span v-if="!isDayOff" class="dividers" v-language:inner="'calendar_to'"/>

                    <v-select v-if="!isDayOff" class="select-cal" height="30" dense tag="span"
                                v-model="selectedHourTo"
                                :append-icon="'sbf-arrow-down'" 
                                :items="hoursToList" outline>
                        <template slot="selection" slot-scope="data">{{timeFormat(data.item)}}</template>
                        <template slot="item" slot-scope="item">{{timeFormat(item.item)}}</template>
                    </v-select>
                    <span v-if="(!isMobile && !isDayOff) && (selectedHourTo < 23 && !isAddTimeSlot)" 
                            @click="isAddTimeSlot = true" class="addTime" 
                            v-language:inner="'calendar_add_time'"/>

                    <span v-if="(isMobile && !isDayOff) && (selectedHourTo < 23 && !isAddTimeSlot)" 
                            @click="isAddTimeSlot = true" class="addTime" 
                            v-language:inner="'calendar_add_time_mobile'"/>

                    <span :class="[isMobile?'dividersAnd':'dividers']" 
                            v-show="(!isDayOff && isAddTimeSlot)"
                            v-language:inner="'calendar_and'"/>

                    <v-select v-if="!isDayOff && showAdditional" class="select-cal" height="30" dense
                            v-model="selectedAdditionalHourFrom"
                            :append-icon="'sbf-arrow-down'" 
                            :items="hoursAdditionaFromList" outline>
                        <template slot="selection" slot-scope="data">{{timeFormat(data.item)}}</template>
                        <template slot="item" slot-scope="item">{{timeFormat(item.item)}}</template>
                    </v-select>

                    <span v-if="!isDayOff && showAdditional" class="dividers" 
                          v-language:inner="'calendar_to'"/>

                    <v-select v-if="!isDayOff && showAdditional" :class="['select-cal',{'mt-3':isMobile}]" height="30" dense
                            v-model="selectedAdditionalHourTo"
                            :append-icon="'sbf-arrow-down'" 
                            :items="hoursAdditionaToList" outline>
                        <template slot="selection" slot-scope="data">{{timeFormat(data.item)}}</template>
                        <template slot="item" slot-scope="item">{{timeFormat(item.item)}}</template>
                    </v-select>

                    <removeTimeSVG v-if="!isDayOff && showAdditional" 
                                   @click.native="closeAdditionalTime" 
                                   class="removeTime"/> 
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
        }
    },
    computed: {
        ...mapGetters(['getIntervalFirst']),
        dayName(){
            let options = { weekday: this.isMobile? 'short':'long' }
            let dayDate = new Date(`01-0${this.day+1}-2017`)
            return dayDate.toLocaleString(`${global.lang}-${global.country}`,options )
        },
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        isDayOff(){
            return (this.selectedHourFrom == this.hoursList[0])
        },
        hoursToList(){
            let start = this.selectedHourFrom +1;
            let end = 24;    
            let difference = Math.abs(start-end);
            let rangeArray = new Array(difference + 1).fill(undefined).map((val, key) => {
                return start > end ? start - key : start + key;
            })
            if(this.selectedHourTo <= rangeArray[0]){
                this.selectedHourTo = rangeArray[0]
            }
            return rangeArray
        },

        hoursAdditionaFromList(){
            let listTo = this.hoursList.filter(hour=> hour > this.selectedHourTo)
            if(this.selectedAdditionalHourFrom <= listTo[0]){
                this.selectedAdditionalHourFrom = listTo[0]
            }
            return listTo
        },
        hoursAdditionaToList(){
            let listTo = this.hoursAdditionaFromList.filter(hour=> hour > this.selectedAdditionalHourFrom)
                listTo.push(24)
            if(this.selectedAdditionalHourTo <= listTo[0]){
                this.selectedAdditionalHourTo = listTo[0]
            }
            return listTo
            
        },
        showAdditional(){
            return ( this.isAddTimeSlot && this.selectedHourTo < 24)
        }, 
    },
    methods: {
        ...mapActions(['updateStateAvailabilityCalendar']),
        timeFormat(time){
            if(time == this.hoursList[0]) {
                return time
            }else{
                return (time < 10) ? `0${time}:00` : `${time}:00`;
            }
        },
        closeAdditionalTime(){
            this.isAddTimeSlot = false;
            this.selectedAdditionalHourFrom = ''
            this.selectedAdditionalHourTo = ''
        },
        updateAvailability(){
            let timeFrames = [
                    `${this.timeFormat(this.selectedHourFrom)}:00`,
                    `${this.timeFormat(this.selectedHourTo)}:00`
                ]

            if(this.isAddTimeSlot){
                let additionalHourTimeFrames = [
                    `${this.timeFormat(this.selectedAdditionalHourFrom)}:00`,
                    `${this.timeFormat(this.selectedAdditionalHourTo)}:00`
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
        }
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
        this.selectedHourFrom = this.getIntervalFirst
    },
    mounted() {
        this.runUpdate()
        this.updateStateAvailabilityCalendar(this.availabilityDay) 
    },
    updated() {
        this.runUpdate()
        this.updateStateAvailabilityCalendar(this.availabilityDay) 
    },
    
}
</script>

<style lang="less">
    @import '../../styles/mixin.less';
    table{
        tr td:first-child{
            padding-right: 6px;
            white-space:nowrap;
            vertical-align: baseline;
            padding-top: 4px;
        }
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
                .dividers{
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
                                    margin-top: 6px !important;
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
    }

</style>