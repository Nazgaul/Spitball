<template>
    <div class="liveSession">

        <v-row class="d-flex ma-0 pa-0 mb-3" dense>
            <v-col cols="12" sm="6" class="pa-0">
                <v-menu ref="datePickerMenu" v-model="datePickerMenu" :close-on-content-click="false" transition="scale-transition" offset-y max-width="290" min-width="290px">
                    <template v-slot:activator="{ on }">
                        <v-text-field 
                            v-on="on"
                            v-model="date"
                            type="text"
                            class="dateInput"
                            :rules="[rules.required]"
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
                        :next-icon="isRtl ? 'sbf-arrow-left-carousel' : 'sbf-arrow-right-carousel'"
                        :prev-icon="isRtl ? 'sbf-arrow-right-carousel' : 'sbf-arrow-left-carousel'"
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

            <v-col cols="12" sm="6" class="pa-0">
                <v-select
                    v-model="hour"
                    class="roomType pl-sm-4"
                    :items="timeHoursList"
                    height="50"
                    :menu-props="{
                        maxHeight: 200
                    }"
                    :label="$t('dashboardPage_labe_hours')"
                    prepend-inner-icon="sbf-clockIcon"
                    append-icon="sbf-menu-down"
                    color="#304FFE"
                    placeholder=" "
                    dense
                    outlined
                ></v-select>
            </v-col>
        </v-row>

        <v-text-field 
            v-model="liveSessionTitle"
            type="text"
            class="sessionTitleInput mb-3"
            :rules="[rules.required]"
            :label="$t('dashboardPage_label_live_title')"
            height="50"
            color="#304FFE"
            dense
            outlined
            placeholder=" "
            autocomplete="nope"
        >
        </v-text-field>

        <v-textarea
            v-model="sessionAboutText"
            class="sessionAbout mb-3"
            :rules="[rules.required]"
            :label="$t('dashboardPage_label_session_about')"
            :rows="3"
            color="#304FFE"
            placeholder=" "
            dense
            outlined
            no-resize
        ></v-textarea>

        <div>

            <v-row class="align-center pa-0 ma-0 mb-5" dense>
                <v-col cols="12" sm="4" class="pa-0">
                    <div class="priceTitle mb-2 mb-sm-0" v-t="'dashboardPage_visitor_price'"></div>
                </v-col>
                <v-col cols="12" sm="4" class="pa-0" >
                    <v-select
                        v-model="currentVisitorPriceSelect"
                        :items="items"
                        class="selectVisitorPrice mb-6 mb-sm-0"
                        color="#304FFE"
                        height="50"
                        dense
                        append-icon="sbf-menu-down"
                        return-object
                        hide-details
                        outlined
                    ></v-select>
                </v-col>
                <v-col cols="6" sm="4" class="pa-0" v-if="currentVisitorPriceSelect.value === 'price'">
                    <v-text-field 
                        v-model="myPrice"
                        type="number"
                        class="pl-sm-5 roomPrice"
                        color="#304FFE"
                        :rules="[rules.required,rules.minimum]"
                        :label="$t('becomeTutor_placeholder_price', {'0' : getSymbol})"
                        :placeholder="$t('becomeTutor_placeholder_price', {'0' : getSymbol})"
                        height="50"
                        hide-details
                        outlined
                    >
                </v-text-field>
                </v-col>
            </v-row>

            <v-row class="align-center pa-0 ma-0">
                <v-col cols="8" sm="4" class="pa-0">
                    <div class="priceTitle" v-t="'dashboardPage_subscription_price'"></div>
                </v-col>
                <v-col cols="4" sm="4" class="pa-0">
                    <div class="ml-4 ml-sm-0 priceSubscription" v-t="'dashboardPage_subscription_free'"></div>
                </v-col>
            </v-row>

        </div>
    </div>
</template>

<script>
import { validationRules } from '../../../../../services/utilities/formValidationRules.js'

export default {
    name: 'liveSession',
    props: {
        price: {
            required: true
        },
        currentError: {
            type: String,
        },
    },
    data() {
        return {
            isRtl: global.isRtl,
            liveSessionTitle: '',
            sessionAboutText: '',
            date: new Date().FormatDateToString(),
            hour: '00:00 AM',
            datePickerMenu: false,
            currentVisitorPriceSelect: { text: this.$t('dashboardPage_visitors_free'), value: 'free' },
            items: [
                { text: this.$t('dashboardPage_visitors_free'), value: 'free' },
                { text: this.$t('dashboardPage_visitors_set_price'), value: 'price' }
            ],
            rules: {
                required: (value) => validationRules.required(value),
                minimum: (value) => validationRules.minVal(value,0),
            }
        }
    },
    watch: {
        date(val) {
            this.resetErrors(val)
        },
        hour(val) {
            this.resetErrors(val)
        },
        liveSessionTitle(val) {
            this.resetErrors(val)
        },
        sessionAboutText(val) {
            this.resetErrors(val)
        },
        currentVisitorPriceSelect(val) {
            this.resetErrors(val)
        }
    },
    computed: {
        myPrice: {
            get() {
                return this.price
            },
            set(price) {
                this.$emit('updatePrice', price)
            }
        },
        timeHoursList() {
            let timesArr = [], i
            for(i = 0; i < 60 * 24; i = i + 15) {
                let label = parseInt(i / (60 * 12)) === 0 ? "AM" : "PM";
                let h = parseInt(i / 60);
                let m = i % 60;
                let time = `${h.toString().padStart(2, '0')}:${m.toString().padStart(2, '0')} ${label}`
                timesArr.push(time)
            }
            return timesArr
        },
        getSymbol() {
            let v = this.$n(1, 'currency');
            return v.replace(/\d|[.,]/g,'').trim();
        },
    },
    methods: {
        allowedDates(date) {
            let today = new Date().FormatDateToString()
            return date >= today
        },
        resetErrors(val) {
            if(val && this.currentError) {
                this.$emit('resetErrors')
            }
        }
    }
}
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';
@import '../../../../../styles/colors.less';

.liveSession {
    .v-text-field__slot{
        input{
            margin: 4px 0 0 4px;
        }
    }
    .dateInput, .roomType, .roomPrice, .priceTitle, .sessionTitleInput, .sessionAbout, .sbf-menu-down {
        input, textarea {
            color: @global-purple !important;
        }
    }
    .priceSubscription, .v-select__selection--comma {
        color: @global-purple !important;
    }
    .v-input__prepend-inner, .v-input__append-inner{
        margin-top: 14px !important; // vuetify icons inside input
    }
    .selectVisitorPrice  {
        .v-select__selection--comma {
            line-height: normal !important; // vuetify line height issue
        }
    }
    .priceTitle {
        font-size: 16px;
        font-weight: 600;
    }
    .sbf-menu-down {
        font-size: 34px;
    }
}
</style>