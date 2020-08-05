<template>
    <div class="courseTeaching mx-5 pt-5" :class="{'pb-5': numberOfLecture === index}">
        <div class="lectureTitle d-flex align-center justify-space-between mb-6">
            <div>{{$t('live_lecture', [index])}}</div>
            <v-icon @click="removeLecture" size="12" color="grey">{{$vuetify.icons.values.close}}</v-icon>
        </div>

        <div class="mb-2">
            <v-text-field 
                v-model="lectureTopic"
                :label="$t('lecture_topic')"
                :placeholder="$t('lecture_topic')"
                class="lectureText"
                height="50"
                color="#304FFE"
                autocomplete="off"
                dense
                outlined
            >
            </v-text-field>
        </div>

        <v-row class="sessionDetails ma-0 pa-0 mb-3 flex-wrap" no-gutters>
            <v-col cols="6" md="4" >
                <v-menu ref="datePickerMenu" v-model="datePickerMenu" :close-on-content-click="false" transition="scale-transition" offset-y max-width="290" min-width="290px">
                    <template v-slot:activator="{ on }">
                        <v-text-field 
                            v-on="on"
                            v-model="date"
                            type="text"
                            class="dateInput"
                            :label="$t('dashboardPage_label_date')"
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
            <v-col cols="6" md="4" >
                <v-select
                    v-model="hour"
                    class="roomHour ps-sm-5"
                    :items="timeHoursList"
                    menu-props="auto"
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
            rules: {
                required: (value) => validationRules.required(value),
                minimum: (value) => validationRules.minVal(value,0),
                integer: (value) => validationRules.integer(value),
            }
        }
    },
    watch: {
        date: {
            immediate: true,
            handler(date) {
                this.$store.commit('setTeachLecture', {
                    index: this.index-1,
                    date
                })
            }
        },
        hour: {
            immediate: true,
            handler(hour) {
                this.$store.commit('setTeachLecture', {
                    index: this.index-1,
                    hour
                })
            }
        },
    },
    computed: {
        lectureTopic: {
            get() {
                return this.$store.getters.getTeachLecture[this.index-1]?.text || ''
            },
            set(text) {
                this.$store.commit('setTextLecture', {
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
        numberOfLecture() {
            return this.$store.getters.getNumberOfLecture
        },
    },
    methods: {
        allowedDates(date) {
            let today = new Date().FormatDateToString()
            return date >= today
        },
        // addLecture() {
        //     this.$store.commit('setNumberOfLecture', this.numberOfLecture + 1)
        // },
        removeLecture() {
            this.$store.commit('removeLecture', this.index)
        }
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.courseTeaching {
    max-width: 760px;

    &:not(:nth-last-child(2)) {
        border-bottom: 1px solid #dddddd;
    }
    .lectureTitle {
        font-size: 18px;
        font-weight: 600;
        color: @global-purple;
    }
    .lectureText {
        max-width: 560px;
    }
    .v-input__prepend-inner, .v-input__append-inner {
        margin-top: 14px !important;
    }
}
</style>