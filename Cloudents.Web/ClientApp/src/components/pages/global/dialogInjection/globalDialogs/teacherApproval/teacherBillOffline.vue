<template>
   <v-dialog :value="true" persistent :maxWidth="'580'" :content-class="'teacherBillOffline'" :fullscreen="$vuetify.breakpoint.xsOnly">
        <div class="py-4 pa-sm-4 text-center wrapper">
            <div>
                <div class="text-right pe-4 pe-sm-0 d-sm-none"><v-icon size="12" @click="closeDialog()">sbf-close</v-icon></div>

                <div class="mainTitle text-center" :class="[modifyDurationError || priceError ? 'mb-3' : 'mb-12']" v-t="'teacherApproval_title'"></div>

                <div class="v-alert green tableEmptyState text-left mb-5 pa-2 align-start align-sm-center" v-if="showSuccess">
                    <span class="white--text" v-t="'teacherApproval_success'"></span>
                </div>
                <div class="v-alert error tableEmptyState text-left mb-5 pa-2 align-start align-sm-center" v-if="modifyDurationError">
                    <whiteWarn class="image me-2 me-sm-4 pt-1 pt-sm-0" width="50" />
                    <span class="white--text" v-t="'teacherApproval_error'"></span>
                </div>
                <div class="v-alert error tableEmptyState text-left mb-5 pa-2 align-start align-sm-center" v-if="priceError">
                    <whiteWarn class="image me-2 me-sm-4 pt-1 pt-sm-0" width="30"/>
                    <span class="white--text" v-t="'teacherApproval_error_price'"></span>
                </div>

                <table class="table text-start">
                    <tr>
                        <td>
                            <div class="pb-3" v-t="'teacherApproval_date'"></div>
                        </td>
                        <td>
                            <div class="mb-3 ps-2">{{formatDate}}</div>
                        </td>
                    </tr>

                    <tr class="studentRow">
                        <td>
                            <div class="pb-9" v-t="'teacherApproval_student'"></div>
                        </td>
                        <td>
                            <v-form class="userName" ref="selectUser">
                                <v-select 
                                    rounded flat outlined
                                    :append-icon="'sbf-arrow-fill'" 
                                    height="30" dense
                                    :items="myFollowers"
                                    item-text="name"
                                    solo
                                    v-model="selectedStudent"
                                    return-object
                                    :label="$t('teacherApproval_select_student')"
                                    :rules="[rules.required]"
                                ></v-select>
                            </v-form>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <div class="pt-1" v-t="'teacherApproval_session_duration'"></div>
                        </td>
                        <td>
                            <div class="d-flex align-center">
                                <input type="number" class="durationInput" maxlength="4" @keypress="inputRestriction" v-model.number="newSessionDuration" />
                                <span class="ms-2" v-t="'teacherApproval_minutes'"></span>
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <div class="py-4" v-t="'teacherApproval_lesson_per_hour'"></div>
                        </td>
                        <td>
                            <input type="number" class="durationInput" maxlength="4" @keypress="inputRestriction" v-model.number="tutorPricePerHour" />
                            <i18n-n :value="0" :format="'currency'">
                                <template v-slot:currency="slotProps"><span>{{slotProps.currency}}</span></template>
                                <!-- Dont show the integer value only the currency symbol -->
                                <template v-slot:integer></template>
                            </i18n-n>
                        </td>
                    </tr>

                    <tr class="bordeTop font-weight-bold">
                        <td class="pt-4"><div class="totalText" v-t="'teacherApproval_total_session'"></div></td>
                        <!-- TODO: Currency Change -->
                        <td class="pt-4 ps-2"><div class="totalNumber">{{$n(totalPrice, {'style':'currency','currency': currencySymbol, minimumFractionDigits: 0, maximumFractionDigits: 0})}}</div></td>
                    </tr>
                </table>
            </div>

            <div class="d-flex bottom px-4 px-sm-0">
                <v-btn icon color="#5A66FF" @click="openIntercom" :ripples="false" depressed><needHelpIcon/></v-btn>

                <div class="bottomActions d-flex text-center">
                    <v-btn width="140" height="40" color="#4452fc" class="d-none d-sm-block me-3" rounded outlined @click="closeDialog()">{{$t('teacherApproval_btn_cancel')}}</v-btn>
                    <v-btn :loading="isLoading" width="140" height="40" color="#4452fc" class="white--text" @click="approveSession" rounded depressed>{{$t('teacherApproval_btn_approve')}}</v-btn>
                </div>
            </div>

        </div>
    </v-dialog>
</template>

<script>
import intercomService from "../../../../../../services/intercomService"
import needHelpIcon from './need_help.svg'
import whiteWarn from './whiteWarn.svg'
import { validationRules } from '../../../../../../services/utilities/formValidationRules'

export default {
    name: 'teacherBillOffline',
    components: {
        needHelpIcon,
        whiteWarn
    },
    data() {
        return {
            session: {
                tutorPricePerHour:0,
            },
            isLoading:false,
            showSuccess:false,
            totalPrice: 0,
            newSessionDuration: 0,
            modifyDurationError: false,
            priceError:false,
            MAX_DIGITS: 9999,
            myFollowers:[],
            selectedStudent:null,
            rules: {
                required: (value) => validationRules.required(value),
            },
        }
    },
    watch: {
        newSessionDuration(val) {
            this.updateTotalPrice(val)
        }
    },
    computed: {
        currencySymbol() {
            return this.$store.getters.accountUser?.currencySymbol
        },
        formatDate() {
            return this.$d(new Date())
        },
        tutorPricePerHour: {
            get() {
                return Math.floor(this.session.tutorPricePerHour)
            },
            set(val) {
                this.session.tutorPricePerHour = val
                this.updateTotalPrice(this.newSessionDuration)
                if(this.session.tutorPricePerHour <= 0){
                    this.priceError = true;
                }else{
                    this.priceError = false;
                }
            }
        }
    },
    methods: {
        closeDialog(){
            if(this.$store.getters.getPendingPayment > 0) {
                this.$store.commit('setComponent', 'linkToaster');
            }else{
                this.$store.commit('setComponent', '')
            }
        },
        approveSession() {
            this.modifyDurationError = false;
            this.priceError = false;
            if(this.newSessionDuration <= 0) {
                this.modifyDurationError = true;
                return
            }
            if(!this.$refs.selectUser.validate()){
                return
            }
            if(this.session.tutorPricePerHour <= 0){
                this.priceError = true;
                return
            }
            let sessionParams = {
                userId: this.selectedStudent.userId,
                durationInMinutes: this.newSessionDuration,
                price: this.session.tutorPricePerHour
            }
            let self = this
            this.isLoading = true;
            this.$store.dispatch('updateBillOffline',sessionParams)
                .then(()=>{
                    self.showSuccess = true;
                    setTimeout(() => {
                        self.isLoading = false;
                        self.closeDialog();
                    }, 3000);
                }).catch(ex => {
                    self.isLoading = false;
                    self.$appInsights.trackException(ex)
                })
        },
        updateTotalPrice(duration) {
            this.totalPrice = this.session.tutorPricePerHour * duration / 60;
        },
        inputRestriction(e) {
            this.modifyDurationError = false;
            if (!/\d/.test(e.key)) {
                e.preventDefault();
            }
            var x = parseInt(this.newSessionDuration + e.key,10);
            if (x >= this.MAX_DIGITS) {
                e.preventDefault();
                
            }
        },
        openIntercom() {
            intercomService.showDialog();
        }
    },
    created() {
        this.$store.dispatch('updateFollowersItems').then(()=>{
            this.myFollowers = this.$store.getters.getFollowersItems.filter(u=>u.hasCreditCard);
        })
    },
}
</script>

<style lang="less">

@import '../../../../../../styles/mixin.less';
.teacherBillOffline{
    background: #fff;

    .wrapper {
        @media (max-width: @screen-xs) {
            height: 100%;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }  
    }
    .mainTitle {
        color: @global-purple;
        font-size: 20px;
        font-weight: 600;
    }

    .tableEmptyState {
        font-size: 14px;
        @media (max-width: @screen-xs) {
            border-radius: 0;
            font-size: 12px !important;
        }  
        .image {
            @media (max-width: @screen-xs) {
                width: 60px;
            }                
        }
    }

    .table {
        color: @global-purple;
        border-spacing: 0;
        margin: 0 auto;
        font-size: 16px;
        @media (max-width: @screen-xs) {
            font-size: 14px;
        }
        .durationInput {
            background: rgba(184, 192, 209, .2);
            width: 55px;
            padding: 6px 8px;
            outline: none;
            text-align: left;
            -moz-appearance: none;
             appearance: none;
            &::-webkit-inner-spin-button, 
            &::-webkit-outer-spin-button { 
                -webkit-appearance: none;
                margin: 0; 
            }

        }
        .bordeTop td{
            border-top: 1px solid #898899;
            .totalNumber {
                font-size: 20px;
            }
        }
        .studentRow {
            vertical-align: sub;
            .userName {
                max-width: 200px;
                @media (max-width: @screen-xs) {
                    max-width: 130px;
                }
                .v-text-field__details{
                    margin-bottom: 0;
                }
                .v-input__slot{
                    padding-left: 12px;
                    padding-right: 10px;
                }
                .v-input__icon{
                    i{
                        font-size: 6px;
                        color: #43425d;
                    }
                }
            }
        }
        tr td:first-child {
            padding-right: 14px;
        }
    }
    .bottom {
        margin-top: 70px;
        .bottomActions {
            width: 100%;
            justify-content: center;
        }
    }
}
</style>