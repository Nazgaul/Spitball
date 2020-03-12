<template>
   <v-dialog :value="true" persistent :maxWidth="'580'" :content-class="'teacherApproval'" :fullscreen="$vuetify.breakpoint.xsOnly">
        <div class="py-4 pa-sm-4 text-center wrapper">
            <div class="">
                <div class="text-right pr-4 pr-sm-0 d-sm-none"><v-icon size="12" v-closeDialog>sbf-close</v-icon></div>

                <div class="mainTitle text-center" :class="[modifyDurationError ? 'mb-3' : 'mb-12']">
                    {{$t('teacherApproval_title')}}
                </div>

                <div class="v-alert error tableEmptyState text-left mb-5 pa-2 align-start align-sm-center" v-if="modifyDurationError">
                    <whiteWarn class="mr-2 mr-sm-4 pt-1 pt-sm-0" width="60"/>
                    <span class="white--text">{{$t('teacherApproval_error')}}</span>
                </div>

                <table class="table text-left">
                    <tr>
                        <td>
                            <div class="pb-3">{{$t('teacherApproval_date')}}</div>
                        </td>
                        <td>
                            <div class="mb-3">{{formatDate}}</div>
                        </td>
                    </tr>

                    <tr class="studentRow">
                        <td>
                            <div class="pb-9">{{$t('teacherApproval_student')}}</div>
                        </td>
                        <td>
                            <div class="mb-9 userName">{{session.name}}</div>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <div  class="pt-1">{{$t('teacherApproval_session_duration')}}</div>
                        </td>
                        <td>
                            <div class="d-flex align-center">
                                <input type="text" class="durationInput text-center" v-model="sessionDuration" />
                                <span class="ml-2">{{$t('teacherApproval_minutes')}}</span>

                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <div class="py-4">{{$t('teacherApproval_lesson_per_hour')}}</div>
                        </td>
                        <td>
                            <div class="py-4">{{$n(session.tutorPricePerHour, 'currency')}}</div>
                        </td>
                    </tr>

                    <tr v-if="session.couponTutor">
                        <td class="pb-4">
                            <div>{{$t('teacherApproval_coupon_discount')}}</div>
                        </td>
                        <td class="pb-4">
                            <div>- {{$n(session.couponValue, 'currency')}} ({{session.couponCode}})</div>
                        </td>
                    </tr>

                    <tr class="bordeTop font-weight-bold">
                        <td class="pt-4"><div class="totalText">{{$t('teacherApproval_total_session')}}</div></td>
                        <td class="pt-4"><div class="totalNumber">{{$n(totalPrice, 'currency')}}</div></td>
                    </tr>
                </table>
            </div>

            <div class="d-flex bottom px-4 px-sm-0">
                <v-btn icon color="#5A66FF" @click="openIntercom" :ripples="false" depressed><needHelpIcon/></v-btn>

                <div class="bottomActions d-flex text-center">
                    <v-btn width="140" height="40" color="#4452fc" class="d-none d-sm-block mr-3" rounded outlined v-closeDialog>{{$t('teacherApproval_btn_cancel')}}</v-btn>
                    <v-btn width="140" height="40" color="#4452fc" class="pb-1 white--text" @click="approveSession" rounded depressed>{{$t('teacherApproval_btn_approve')}}</v-btn>
                </div>
            </div>

        </div>
    </v-dialog>
</template>

<script>
import intercomService from "../../../../../../services/intercomService"
import needHelpIcon from './need_help.svg'
import whiteWarn from './whiteWarn.svg'

export default {
    name: 'teacherApproval',
    components: {
        needHelpIcon,
        whiteWarn
    },
    data() {
        return {
            session: {},
            totalPrice: 0,
            newSessionDuration: null,
            modifyDurationError: false,
            MAX_MINUTES: "10"
        }
    },
    computed: {
        formatDate() {
            if(this.session.date) {
                return this.$d(new Date(this.session.date))
            }
            return ''
        },
        sessionDuration: {
            get() {
                if(this.newSessionDuration) {
                    return this.newSessionDuration
                }
                return this.session.totalMinutes
            },
            set(newVal) {
                this.updateNewSessionDuration(newVal);
            }
        }
    },
    methods: {
        updateNewSessionDuration(duration) {
            this.modifyDurationError = false;
            this.newSessionDuration = duration;
            this.updateTotalPrice(duration)
        },
        approveSession() {
            if(this.newSessionDuration > this.session.totalMinutes || this.newSessionDuration < this.MAX_MINUTES) {
                this.modifyDurationError = true;
                return
            }
            
            let newSessionDuration = {
                sessionId: this.session.sessionId,
                realDuration: this.newSessionDuration
            }
            let self = this
            this.$store.dispatch('updateSessionDuration', newSessionDuration).then(res => {
                console.log(res);
            }).catch(ex => {
                self.$appInsights.trackException({exception: new Error(ex)});
            }).finally(() => {
                self.$closeDialog()
            })
        },
        updateTotalPrice(duration) {
            let total;
            if(this.session.couponCode) {
                if(this.session.couponType === 'Flat') {
                    total = Math.max((this.session.tutorPricePerHour * duration / 60) - this.session.couponValue, 0)
                } else {
                    total = (this.session.tutorPricePerHour * duration / 60) * (1 - (this.session.couponValue / 100))
                }
            } else {
                total = this.session.tutorPricePerHour * duration / 60
            }            
            this.totalPrice = total;
        },
        openIntercom() {
            intercomService.showDialog();
        }
    },
    mounted() {
        let self = this;
        let item = this.$route.params.item
        this.$store.dispatch('updateSalesSessions', item?.sessionId).then(session => {
            self.session = {...session, ...item};
            self.updateTotalPrice(item.totalMinutes)
        })
    }
}
</script>

<style lang="less">

@import '../../../../../../styles/mixin.less';
.teacherApproval{
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
                width: 20px;
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
            width: 50px;
            padding: 6px 8px;
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
                @media (max-width: @screen-xs) {
                    max-width: 130px;
                }
            }
        }
        tr td:first-child {
            padding-right: 10px;
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