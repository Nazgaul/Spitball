<template>
    <v-dialog :value="true" :content-class="'coupon-dialog'" persistent max-width="410px" :fullscreen="$vuetify.breakpoint.xsOnly">
        <v-card class="pb-4 coupon-dialog-card" :class="{'d-block': $vuetify.breakpoint.xsOnly}">
            <v-layout class="header py-6">
                <v-flex
                    class="text-xs-center coupon-dialog-header"
                    :class="{'mt-5': $vuetify.breakpoint.xsOnly}"
                >
                    <span v-t="'coupon_title'"></span>
                    <v-icon @click="closeCouponDialog" class="coupon-close" v-html="'sbf-close'" />
                </v-flex>
            </v-layout>
            <v-layout class="px-4" column>
                <v-flex class="mb-2">
                    <div class="coupon coupon__dialog">
                        <div class="text-xs-right">
                            <div class="coupon__dialog--flex">
                                <input
                                    type="text"
                                    @keyup.enter="applyCoupon"
                                    v-model="coupon"
                                    :placeholder="$t('coupon_placeholder')"
                                    class="profile-coupon_input"
                                    autofocus
                                />
                                <button
                                    class="profile-coupon_btn white--text"
                                    :disabled="disableApplyBtn"
                                    @click="applyCoupon"
                                    v-t="'coupon_apply_btn'"
                                ></button>
                            </div>
                            <div
                                class="profile-coupon_error"
                                v-if="isCouponError"
                            >
                            {{couponErrorText}}
                            </div>
                        </div>
                    </div>
                </v-flex>
            </v-layout>
        </v-card>
    </v-dialog>
</template>

<script>
import storeService from '../../../../services/store/storeService';
import couponStore from '../../../../store/couponStore';
import * as routeNames from '../../../../routes/routeNames';
import EventBus from '../../../../eventBus.js';

export default {
    name: 'applyCoupon',
    data() {
        return {
            couponErrorText: '',
            coupon: '',
            disableApplyBtn: false,
        }
    },
    watch: {
        coupon(val) {
            if(val && this.isCouponError) {
                this.$store.commit('setCouponError', false)
            }
        },
    },
    computed: {
        roomTutorId() {
            if(this.$route.name === routeNames.CoursePage){
                return this.$store.getters.getCourseDetails?.tutorId;
            }else{
                return this.$store.getters.getRoomTutor?.tutorId;
            }
        },
        isCouponError() {
            return this.$store.getters.getCouponError
        }
    },
    methods: {
        applyCoupon() {
            this.disableApplyBtn = true;
            let tutorId = this.roomTutorId;
            let roomId = this.$route.params.id;
            let coupon = this.coupon;
            let self = this;
            this.$store.dispatch('updateCoupon', {coupon, tutorId, roomId})
            .then(() => {
                EventBus.$emit('applyCouponDone');
                self.closeCouponDialog();
            })
            .catch(err => {
                if (err?.response.status === 409) {
                    this.couponErrorText = this.$t('coupon_course_appliy')
                } else {
                    this.couponErrorText = this.$t('coupon_apply_error')
                }
                this.$store.commit('setCouponError', true);
            })
            .finally(() => {
                self.coupon = ''
                self.disableApplyBtn = false;
                if(!self.getCouponError) {
                    this.$ga.event('Tutor_Engagement', 'Redeem_Coupon_Success', `${this.$route.path}`);
                }
            })
        },
        closeCouponDialog() {
            this.coupon = ''
            this.$store.commit('setCouponError', '')
            this.$store.commit('setComponent')
        },
    },
    beforeDestroy(){
        storeService.unregisterModule(this.$store, 'couponStore');
     },
    created() {
        storeService.registerModule(this.$store, 'couponStore', couponStore);
    }
}
</script>

<style lang="less">
.coupon-dialog {
  border-radius: 6px;
  background: white;
  display: flex;
  flex-direction: column;
  .coupon-dialog-header {
    text-align: center;
    font-weight: 600;
    font-size: 18px;
    color: #43425d;
    .coupon-close {
      position: absolute;
      right: 10px;
      top: 10px;
      font-size: 12px;
      fill: #adadba;
      cursor: pointer;
    }
  }
  .coupon {
    display: flex;
    width: 100%;
    justify-content: center;
    &__dialog {
      &--flex {
        display: flex;
      }
      .profile-coupon_input {
        outline: none;
        border-radius: 6px 0 0 6px;
        width: 200px;
        border: 1px solid #bbb;
        padding: 10px 2px 11px 8px;
        margin-right: -5px;
      }
      .profile-coupon_btn {
        border-radius: 0 6px 6px 0;
        background-color: #4c59ff;
        font-size: 12px;
        padding: 8px 20px;
        font-weight: 600;
        outline: none;
      }
      .profile-coupon_error {
        width: 236px;
        margin-top: 4px;
        text-align: left;
        font-size: 11px;
        color: #ff5252;
      }
    }
  }
}
</style>