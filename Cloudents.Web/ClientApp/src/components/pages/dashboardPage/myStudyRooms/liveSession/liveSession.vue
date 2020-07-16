<template>
    <div class="liveSession">
        <div class="liveSubtitle mb-7" v-t="'session details'"></div>


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
        <v-row class="sessionDetails  ma-0 pa-0 mb-3"  no-gutters>
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
            <v-col cols="6" sm="3" >
                <v-select
                    v-model="hour"
                    class="roomHour ps-sm-3 ps-2"
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
            <v-col cols="12" sm="5" >
                <v-select
                    v-model="currentRepeatItem"
                    class="roomHour ps-sm-3 mt-3 mt-sm-0"
                    :items="repeatItems"
                    height="50"
                    :menu-props="{
                        maxHeight: 200
                    }"
                    :label="$t('repeat label')"
                    prepend-inner-icon="sbf-repeat"
                    append-icon="sbf-menu-down"
                    return-object
                    color="#304FFE"
                    placeholder=" "
                    dense
                    outlined
                ></v-select>
            </v-col>
        </v-row>

            
            <div class="sessionDetails" v-if="currentRepeatItem.value !== 'none'">
                <div  class="sessionRepeat flex-wrap d-sm-flex align-center mb-5 mb-sm-2" v-if="currentRepeatItem.value === 'custom'">
                    <div class="labelWidth mb-4 mb-sm-0" v-t="'repeat'"></div>
                    <div class="d-flex flex-wrap flex-sm-nowrap">
                        <v-checkbox 
                            v-model="repeatCheckbox"
                            v-for="(day, index) in filterDaysOfWeek"
                            class="me-2"
                            :key="index"
                            :disabled="currentRepeatDayOfTheWeek === index"
                            :label="day"
                            :value="index"
                            hide-details
                        ></v-checkbox>
                    </div>
                </div>

                <div  class="sessionEnd d-sm-flex">
                    <div class="labelWidth mb-4 mb-sm-0" v-t="'ends'"></div>
                    <v-radio-group v-model="radioEnd" class="mt-0" row>
                        <v-radio class="mb-3" value="on" sel="datePicker">
                            <template v-slot:label>
                                <span class="sessionOn">{{$t('on')}}</span>
                                <div @click.stop.prevent="">
                                    <v-menu 
                                        ref="datePickerOcurrence"
                                        v-model="datePickerOcurrence"
                                        :close-on-content-click="false"
                                        transition="scale-transition"
                                        offset-y
                                        max-width="290"
                                        min-width="290px"
                                        :disabled="radioEnd !== 'on'"
                                    >
                                        <template v-slot:activator="{ on }">
                                            <v-text-field 
                                                v-on="on"
                                                v-model="dateOcurrence"
                                                type="text"
                                                class="dateInput dateInputEnds"
                                                :rules="[rules.required]"
                                                height="36"
                                                hide-details
                                                prepend-inner-icon="sbf-dateIcon"
                                                color="#304FFE"
                                                autocomplete="nope"
                                                dense
                                                :disabled="radioEnd !== 'on'"
                                                outlined
                                                readonly
                                            >
                                            </v-text-field>
                                        </template>
                                        <v-date-picker
                                            v-model="dateOcurrence"
                                            :disabled="radioEnd !== 'on'"
                                            class="date-picker"
                                            @input="datePickerOcurrence = false"
                                            :allowed-dates="allowedDatesEnd"
                                            :next-icon="isRtl ? 'sbf-arrow-left-carousel' : 'sbf-arrow-right-carousel'"
                                            :prev-icon="isRtl ? 'sbf-arrow-right-carousel' : 'sbf-arrow-left-carousel'"
                                            color="#4C59FF"
                                            dense
                                            no-title
                                        >
                                            <v-spacer></v-spacer>
                                            <v-btn text class="font-weight-bold" color="#4C59FF" @click="datePickerOcurrence = false">{{$t('coupon_btn_calendar_cancel')}}</v-btn>
                                            <v-btn text class="font-weight-bold" color="#4C59FF" @click="$refs.datePickerOcurrence.save(dateOcurrence)">{{$t('coupon_btn_calendar_ok')}}</v-btn>
                                        </v-date-picker>
                                    </v-menu>
                                </div>
                            </template>
                        </v-radio>
                        <v-radio value="after">
                            <template v-slot:label>
                                <span class="sessionAfter">
                                    {{$t('after')}}
                                </span>
                                <div @click.stop.prevent="" class="d-flex align-center">
                                    <v-text-field
                                        v-model.number="endAfterOccurrences"
                                        @keypress="inputRestriction"
                                        maxlength="4"
                                        :rules="[rules.integer]"
                                        class="afterOccurrences pe-2"
                                        color="#304FFE"
                                        outlined
                                        :disabled="radioEnd === 'on'"
                                        autocomplete="off"
                                        hide-details
                                        dense
                                        height="36"
                                    >
                                    </v-text-field>
                                    <div v-t="'occurrences'"></div>
                                </div>
                            </template>
                        </v-radio>
                    </v-radio-group>
                </div>
            </div>

            <!-- <v-col colse="12" class="pa-0 mt-3">
               
            </v-col> -->
       
         <v-textarea
                    v-model="sessionAboutText"
                    class="sessionAbout pb-2"
                    :rules="[rules.required]"
                    :label="$t('dashboardPage_label_session_about')"
                    :rows="3"
                    color="#304FFE"
                    placeholder=" "
                    dense
                    outlined
                    no-resize
                ></v-textarea>

        <div class="sessionPriceWrap pb-5 mb-5">
            <div class="liveSubtitle mb-4" v-t="'session price'"></div>
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
                        class="ps-sm-5 roomPrice"
                        color="#304FFE"
                        :rules="[rules.required,rules.minimum]"
                        :label="$t('becomeTutor_placeholder_price', {'0' : getSymbol})"
                        dense
                        height="50"
                        hide-details
                        outlined
                    >
                    </v-text-field>
                </v-col>
            </v-row>

            <v-row class="align-center pa-0 ma-0" v-if="isTutorSubscribed">
                <v-col cols="8" sm="4" class="pa-0">
                    <div class="priceTitle" v-t="'dashboardPage_subscription_price'"></div>
                </v-col>
                <v-col cols="4" sm="4" class="pa-0">
                    <v-text-field 
                        class="roomPrice"
                        color="#304FFE"
                        dense
                        :value="$t('free')"
                        readonly
                        disabled
                        height="50"
                        hide-details
                        outlined
                    >
                    </v-text-field>
                </v-col>
                <v-col cols="6" sm="4" class="pa-0">
                    <div class="priceModified ps-sm-5" v-t="'price modified'"></div>
                </v-col>
            </v-row>
        </div>

        <div class="addImage">
            <div class="liveSubtitle mb-4" v-t="'add image'"></div>

            <div class="liveImageWrap text-center d-flex flex-column align-center">
                <uploadImage
                    v-show="true"
                    :fromLiveSession="true"
                    @setLiveImage="handleLiveImage"
                    class="editLiveImage"
                />
                <img class="liveImage" :src="previewImage || liveImage" width="200" alt="">
                <div class="recommendedImage mt-2" v-t="'image resolution'"></div>
            </div>
        </div>
    </div>
</template>

<script>
import { validationRules } from '../../../../../services/utilities/formValidationRules.js'
import uploadImage from '../../../../new_profile/profileHelpers/profileBio/bioParts/uploadImage/uploadImage.vue';

export default {
    name: 'liveSession',
    components: {
        uploadImage
    },
    props: {
        price: {
            required: true
        },
        currentError: {
            type: String,
        },
    },
    data() {
        var currentTime = new Date();
        var currentHour = currentTime.getHours().toString().padStart(2,'0')
        var currentMinutes = (Math.ceil(currentTime.getMinutes() / 15) * 15);
        if (currentMinutes === 60) {
            currentHour++;
            currentMinutes = 0;
        }
        return {
            isRtl: global.isRtl,
            currentRepeatDayOfTheWeek: new Date().getDay(),
            radioEnd: 'on',
            liveSessionTitle: '',
            sessionAboutText: '',
            date: new Date().FormatDateToString(),
            dateOcurrence: new Date().FormatDateToString(),
            hour: `${currentHour}:${currentMinutes.toString().padStart(2,'0')}`,
            imageLoading: false,
            datePickerMenu: false,
            datePickerOcurrence: false,
            newLiveImage: null,
            endAfterOccurrences: null,
            previewImage: null,
            repeatCheckbox: [],
            daysOfWeek: [0,1,2,3,4,5,6
                // 'Sunday',
                // 'Monday',
                // 'Tuesday',
                // 'Wednesday',
                // 'Thursday',
                // 'Friday',
                // 'Saturday',
            ],
            
            currentVisitorPriceSelect: { text: this.$t('dashboardPage_visitors_free'), value: 'free' },
            items: [
                { text: this.$t('free'), value: 'free' },
                { text: this.$t('dashboardPage_visitors_set_price'), value: 'price' }
            ],
            currentRepeatItem: { text: this.$t('not repeat'), value: 'none' },
            repeatItems: [
                { text: this.$t('not repeat'), value: 'none' },
                { text: this.$t('daily'), value: 'daily' },
                { text: this.$t('weekly'), value: 'weekly' },
                { text: this.$t('custom'), value: 'custom' }
            ],
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
            handler(val) {
                this.currentRepeatDayOfTheWeek = new Date(val).getDay()
                this.repeatCheckbox = [this.daysOfWeek[this.currentRepeatDayOfTheWeek]]
                this.dateOcurrence = val
                this.resetErrors(val)
            }
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
        },
        endAfterOccurrences(val) {
            this.resetErrors(val)
        }
    },
    computed: {
        filterDaysOfWeek() {
            return this.daysOfWeek.map((day, index) => {
                let dayDate = new Date(`2017-01-0${index+1}`)
                // Language hack for display hebrew first letter
                return this.$moment(dayDate).format('dd', global.country)[0];
            })

        },
        liveImage() {
            return this.isMobile ? require('../../../../new_profile/components/profileLiveClasses/live-banner-mobile.png') : require('../../../../new_profile/components/profileLiveClasses/live-banner-desktop.png')
        },
        isTutorSubscribed() {
            return this.$store.getters.getIsTutorSubscription
        },
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
                let h = parseInt(i / 60);
                let m = i % 60;
                let time = `${h.toString().padStart(2, '0')}:${m.toString().padStart(2, '0')}`
                timesArr.push(time)
            }
            return timesArr
        },
        getSymbol() {
            // TODO: Currency Change
            let accountUser = this.$store.getters.accountUser
            let v = this.$n(1, {'style':'currency','currency': accountUser?.currencySymbol});
            return v.replace(/\d|[.,]/g,'').trim();
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
        resetErrors(val) {
            if(val && this.currentError) {
                this.$emit('resetErrors')
            }
        },
        inputRestriction(e) {
            if (!/\d/.test(e.key)) {
                e.preventDefault();
            }
        },
        handleLiveImage(previewImage) {

            if(previewImage) {
                let formData;
                formData = new FormData();
                let file = previewImage[0];
                formData.append("file", file);

                this.$store.dispatch('updateLiveImage', formData).then(({data}) => {
                    this.previewImage = window.URL.createObjectURL(previewImage[0])
                    this.newLiveImage = data.fileName
                })
            }
        }
    },
    created() {
        if(this.isRtl) {
            this.$nextTick(() => {
                document.querySelectorAll('.roomHour .v-label').forEach(elem => {
                    elem.style.right = '-28px'
                })
            })
        }
        
    }
}
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';
@import '../../../../../styles/colors.less';

.liveSession {
    .liveSubtitle {
        .responsive-property(font-size, 20px, null, 18px);
        font-weight: 600;
        color: @global-purple;
    }
    // .sessionDetails, .sessionPriceWrap {
    //     border-bottom: 1px solid #dddddd;
    // }
    .sessionAbout {
         border-bottom: 1px solid #dddddd;
    }
    .sessionDetails {
        .labelWidth {
            font-size: 16px;
            font-weight: 600;
            color: #43425d;
            width: 80px;
        }
    }
    .v-text-field__slot{
        input{
            margin: 4px 0 0 4px;
        }
    }
    .dateInput, .roomHour, .roomPrice, .priceTitle, .sessionTitleInput, .sessionAbout, .sbf-menu-down {
        input, textarea {
            color: @global-purple !important;
        }
    }
    .dateInputEnds {
        max-width: 136px;

        .v-input__prepend-inner {
            margin-top: 8px !important;
        }
        .v-text-field__slot {
            input {
                margin: 0;
            }
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
    .priceModified {
        color: #595475;
    }
    .sbf-menu-down {
        font-size: 34px;
    }
    .sessionRepeat {
        .v-input--selection-controls {
            margin-top: 0;
        }
    }
    .sessionEnd {
        .sessionOn, .sessionAfter {
            width: 60px;
        }
        .afterOccurrences {
            width: 60px;
        }
    }
    .liveImageWrap {
        position: relative;
        width: max-content;
        margin: 0 auto;
        .editLiveImage {
            position: absolute;
            text-align: center;
            border-radius: 3px;
            background-color: rgba(0,0,0,.6);
            z-index: 1;
            left: 7px;
        }
        .liveImage {
            border: solid 1px #c6cdda;
            border-radius: 4px;
        }
        .recommendedImage {
            font-size: 16px;
            color: #adb1b4;
        }
    }
}
</style>