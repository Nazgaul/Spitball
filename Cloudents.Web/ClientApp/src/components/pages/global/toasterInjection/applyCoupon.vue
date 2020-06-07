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
                    <div class="coupon coupon__dialog" v-if="isTutor && !isMyProfile">
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
                                v-t="'coupon_apply_error'"
                                v-if="isCouponError"
                            ></div>
                        </div>
                    </div>
                </v-flex>
            </v-layout>
        </v-card>
    </v-dialog>
</template>

<script>
export default {
    name: 'applyCoupon',
    data() {
        return {
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
        isTutor() {
            return this.$store.getters.getIsTeacher
        },
        isMyProfile() {
            return this.$store.getters.getIsMyProfile
        },
        isCouponError() {
            return this.$store.getters.getCouponError
        }
    },
    methods: {
        applyCoupon() {
            if(this.isTutor) {
                this.disableApplyBtn = true;
                let tutorId = this.getProfile.user.id;
                let coupon = this.coupon;
                let self = this
                this.updateCoupon({coupon, tutorId}).finally(() => {
                self.coupon = ''
                self.disableApplyBtn = false;
                if(!self.getCouponError) {
                    this.$ga.event('Tutor_Engagement', 'Redeem_Coupon_Success', `${this.$route.path}`);
                }
                })
            }
        },
        closeCouponDialog() {
            this.coupon = ''
            this.$store.commit('setComponent')
            // this.$store.dispatch('updateCouponDialog', false);
        },
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