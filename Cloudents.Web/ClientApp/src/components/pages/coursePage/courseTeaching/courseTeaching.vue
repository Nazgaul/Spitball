<template>
    <div class="courseTeaching pa-5 mb-6">
        <div class="d-flex align-center justify-space-between mb-11">
            <div class="courseTeachingTitle" v-t="'set_Teaching'"></div>
            <v-switch
                v-model="scheduleSwitch"
                class="pa-0 ma-0"
                :label="$t('schedule_course')"
                hide-details
            ></v-switch>
        </div>

        <div>
            <v-text-field 
                v-model="lectureTopic"
                :rules="[rules.required]"
                :label="$t('lecture_topic')"
                :disabled="!scheduleSwitch"
                height="50"
                color="#304FFE"
                autocomplete="off"
                dense
                outlined
            >
            </v-text-field>
        </div>

        <v-row class="sessionDetails ma-0 pa-0 mb-3" no-gutters>
            <v-col cols="6" sm="4" >
                <v-menu ref="datePickerMenu" v-model="datePickerMenu" :close-on-content-click="false" transition="scale-transition" offset-y max-width="290" min-width="290px">
                    <template v-slot:activator="{ on }">
                        <v-text-field 
                            v-on="on"
                            v-model="date"
                            type="text"
                            class="dateInput"
                            :rules="[rules.required]"
                            :label="$t('dashboardPage_label_date')"
                            :disabled="!scheduleSwitch"
                            height="50"
                            prepend-inner-icon="sbf-dateIcon"
                            color="#304FFE"
                            autocomplete="nope"
                            dense
                            outlined
                            readonly
                        >
                        </v-text-field>
                    </template>
                    <v-date-picker
                        v-model="date"
                        class="date-picker"
                        @input="datePickerMenu = false"
                        :allowed-dates="allowedDates"
                        color="#4C59FF"
                        dense
                        no-title
                    >
                        <v-spacer></v-spacer>
                        <v-btn text class="font-weight-bold" color="#4C59FF" @click="datePickerMenu = false">{{$t('coupon_btn_calendar_cancel')}}</v-btn>
                        <v-btn text class="font-weight-bold" color="#4C59FF" @click="$refs.datePickerMenu.save(date)">{{$t('coupon_btn_calendar_ok')}}</v-btn>
                    </v-date-picker>
                </v-menu>
            </v-col>
            <v-col cols="6" sm="4" >
                <v-select
                    v-model="hour"
                    class="roomHour ps-sm-3 ps-2"
                    :items="timeHoursList"
                    :disabled="!scheduleSwitch"
                    :menu-props="{
                        maxHeight: 200
                    }"
                    :label="$t('dashboardPage_labe_hours')"
                    height="50"
                    prepend-inner-icon="sbf-clockIcon"
                    append-icon="sbf-menu-down"
                    color="#304FFE"
                    placeholder=" "
                    dense
                    outlined
                ></v-select>
            </v-col>
        </v-row>
    </div>
</template>

<script>
import { validationRules } from '../../../../services/utilities/formValidationRules.js'

export default {
    name: 'courseTeaching',
    props: {
        index: {
            type: Number
        }
    },
    data() {
        return {
            datePickerMenu: false,
            scheduleSwitch: false,
            rules: {
                required: (value) => validationRules.required(value),
                minimum: (value) => validationRules.minVal(value,0),
                integer: (value) => validationRules.integer(value),
            }
        }
    },
    computed: {
        lectureTopic: {
            get() {
                return this.$store.getters.getTeachLecture[this.index-1]?.text
            },
            set(text) {
                this.$store.commit('setTeachLecture', {
                    index: this.index-1,
                    text
                })
            }
        },
        date: {
            get() {
                return this.$store.getters.getTeachLecture[this.index-1]?.date || new Date().FormatDateToString()
            },
            set(date) {
                this.$store.commit('setTeachLecture', {
                    index: this.index-1,
                    date
                })
            }
        },
        hour: {
            get() {
                return this.$store.getters.getTeachLecture[this.index-1]?.hour || this.$store.getters.getTeachTime
            },
            set(hour) {
                this.$store.commit('setTeachLecture', {
                    index: this.index-1,
                    hour
                })
            }
        },
        timeHoursList() {
            let timesArr = [], i
            for(i = 0; i < 60 * 24; i = i + 15) {
                let h = parseInt(i / 60);
                let m = i % 60;
                let time = `${h.toString().padStart(2, '0')}:${m.toString().padStart(2, '0')}`
                timesArr.push(time)
            }
            return timesArr
        },
    },
    methods: {
        allowedDates(date) {
            let today = new Date().FormatDateToString()
            return date >= today
        },
        allowedDatesEnd(date) {
            let today = new Date().FormatDateToString()
            return date >= today && date >= this.date
        },
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.courseTeaching {
    background: #fff;
    border-radius: 6px;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    max-width: 760px;

    .courseTeachingTitle {
        font-size: 20px;
        font-weight: 600;
        color: @global-purple;
    }
}
</style>