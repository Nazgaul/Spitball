<template>
    <div class="liveSession">

        <v-row class="d-flex" dense>
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
                            prepend-inner-icon="sbf-calendar"
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
            class="sessionTitleInput mb-2"
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
            class="sessionAbout mb-2"
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
                    <div class="mb-2 mb-sm-0" v-t="'dashboardPage_visitor_price'"></div>
                </v-col>
                <v-col cols="12" sm="4" class="pa-0" >
                    <v-select
                        v-model="currentVisitorPriceSelect"
                        :items="items"
                        class="selectVisitorPrice mb-6 mb-sm-0"
                        label="Outlined style"
                        color="#304FFE"
                        height="50"
                        dense
                        append-icon="sbf-menu-down"
                        return-object
                        hide-details
                        outlined
                    ></v-select>
                </v-col>
                <v-col cols="12" sm="4" class="pa-0" v-if="currentVisitorPriceSelect.value === 'price'">
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
                <v-col sm="4" class="pa-0">
                    <div v-t="'dashboardPage_subscription_price'"></div>
                </v-col>
                <v-col sm="4" class="pa-0">
                    <div class="ml-4" v-t="'dashboardPage_subscription_free'"></div>
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
            liveSessionTitle: '',
            sessionAboutText: '',
            date: new Date().FormatDateToString(),
            hour: '00',
            currentVisitorPriceSelect: { text: this.$t('dashboardPage_visitors_free'), value: 'free' },
            isRtl: global.isRtl,
            datePickerMenu: false,
            items: [
                { text: this.$t('dashboardPage_visitors_free'), value: 'free' },
                { text: this.$t('dashboardPage_visitors_set_price'), value: 'price' }
            ],
            rules: {
                required: (value) => validationRules.required(value),
                minimum: (value) => validationRules.minVal(value,0),
            },
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
            let arr = []
            for (let i = 0; i < 24; i++) {
                arr.push(i.toString().padStart(2, '0'));
            }
            return arr
        },
        getSymbol() {
            let v = this.$n(1,'currency');
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
    .dateInput{
        .v-text-field__slot{
            input{
                margin: 4px 0 0 4px;
            }
        }
        .v-input__prepend-inner{
            margin-top: 12px !important;
        }
    }
}
</style>