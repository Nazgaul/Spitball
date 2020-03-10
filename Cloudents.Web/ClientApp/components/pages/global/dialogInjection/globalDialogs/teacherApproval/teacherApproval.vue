<template>
   <v-dialog :value="true" persistent :maxWidth="'580'" :content-class="'teacherApproval'" :fullscreen="$vuetify.breakpoint.xsOnly">
        <div class="py-4 pa-sm-4 text-center wrapper">
            <div class="">
                <div class="text-right pr-4 pr-sm-0"><v-icon size="12" v-closeDialog>sbf-close</v-icon></div>

                <div class="mainTitle text-center" :class="[modifyDurationError ? 'mb-3' : 'mb-12']">
                    {{$t('teacherApproval_title')}}
                </div>

                <div class="v-alert error tableEmptyState text-left mb-5 pa-2 align-start" v-if="!modifyDurationError">
                    <img class="image mr-2" src="../../../../dashboardPage/images/warning.png" alt="" />
                    <span class="white--text">{{$t('teacherApproval_error')}}</span>
                </div>

                <div class="main px-4 px-sm-0">
                    <div class="d-flex justify-space-between align-center mb-3">
                        <div>{{$t('teacherApproval_date')}}</div>
                        <div>{{formatDate}}</div>
                    </div>
                    <div class="d-flex justify-space-between align-center mb-10">
                        <div>{{$t('teacherApproval_student')}}</div>
                        <div>{{session.name}}</div>
                    </div>

                    <div class="d-flex justify-space-between align-center mb-3">
                        <div>{{$t('teacherApproval_session_duration')}}</div>
                        <div class="d-flex align-center">
                            <input type="text" class="durationInput text-center" v-model="sessionDuration" />
                            <span class="ml-2">{{$t('teacherApproval_minutes')}}</span>
                        </div>
                    </div>
                    <div class="d-flex justify-space-between align-center mb-3">
                        <div>{{$t('teacherApproval_lesson_per_hour')}}</div>
                        <div>{{$n(session.tutorPricePerHour, 'currency')}}</div>
                    </div>
                    <div class="d-flex justify-space-between align-center mb-3" v-if="session.couponTutor">
                        <div>{{$t('teacherApproval_coupon_discount')}}</div>
                        <div>- {{$n(session.couponValue, 'currency')}} ({{session.couponCode}})</div>
                    </div>

                    <v-divider></v-divider>
                    
                    <div class="totalSession d-flex justify-space-between align-center mt-3">
                        <div>{{$t('teacherApproval_total_session')}}</div>
                        <div>{{$n(totalPrice, 'currency')}}</div>
                    </div>
                </div>
            </div>

            <div class="d-flex bottom px-4 px-sm-0">
                <v-btn icon color="#5A66FF" @click="openIntercom" :ripples="false" depressed><needHelpIcon/></v-btn>

                <div class="bottomActions d-flex text-center">
                    <v-btn width="140" height="40" color="#4452fc" class="d-none d-sm-block mr-3" rounded outlined v-closeDialog>{{$t('teacherApproval_btn_cancel')}}</v-btn>
                    <v-btn width="140" height="40" color="#4452fc" @click="approveSession" rounded class="white--text" depressed>{{$t('teacherApproval_btn_approve')}}</v-btn>
                </div>
            </div>

        </div>
    </v-dialog>
</template>

<script>
import intercomService from "../../../../../../services/intercomService"
import needHelpIcon from './need_help.svg'

export default {
    name: 'teacherApproval',
    components: {
        needHelpIcon
    },
    data() {
        return {
            session: {},
            totalPrice: 0,
            newSessionDuration: null,
            modifyDurationError: false
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
            if(this.newSessionDuration > this.session.totalMinutes) {
                this.modifyDurationError = true;
                return
            }
            
            let newSessionDuration = {
                sessionId: this.session.sessionId,
                realDuration: this.newSessionDuration
            }

            this.$store.dispatch('updateSessionDuration', newSessionDuration).then(res => {
                console.log(res);
            }).catch(ex => {
                console.log(ex);
            }).finally(() => {
                this.$closeDialog()
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
    created() {
        let item = this.$route.params.item
        this.$store.dispatch('updateSalesSessions', item.sessionId).then(session => {
            this.session = {...session, ...item};
            this.updateTotalPrice(item.totalMinutes)
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

    .main {
        font-size: 16px;
        max-width: 350px;
        margin: 0 auto;
        color: @global-purple;

        @media (max-width: @screen-xs) {
            font-size: 14px;
        }
        .left, .right {
            display: flex;
            flex-direction: column;
            align-items: end;
        }

        .durationInput {
            background: rgba(184, 192, 209, .2);
            width: 70px;
            padding: 6px;
        }
        
        .totalSession {
            font-weight: bold;
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