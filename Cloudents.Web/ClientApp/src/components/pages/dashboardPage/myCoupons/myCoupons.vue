<template>
    <div class="tableCoupon pa-4 pb-2 pb-sm-0">
        <v-data-table
            :headers="headers"
            :items="coupons"
            class="dataTable"
            :loading="tableLoading"
            :mobile-breakpoint="0"
            :footer-props="{
                showFirstLastPage: false,
                itemsPerPageOptions: [5]
            }">

            <template v-slot:top>
                <div class="tableTop d-flex flex-sm-row flex-column align-sm-center justify-space-between">
                    <div class="myCoupons_title pb-3 pb-sm-0" v-t="'marketing_tableCoupon_title'"></div>
                    <div>
                        <v-btn
                            @click="$store.commit('setComponent', 'createCoupon')"
                            class="couponBtn white--text"
                            depressed
                            rounded
                            :block="$vuetify.breakpoint.xsOnly"
                            color="#5360FC"
                            v-t="'dashboardPage_my_create_coupon'"
                        ></v-btn>
                    </div>
               </div>
            </template>

            <template v-slot:header.code="{header}">
                <span>{{header.text}}</span>
            </template>
            <template v-slot:header.couponType="{header}">
                <span>{{header.text}}</span>
            </template>
            <template v-slot:header.value="{header}">
                <span>{{header.text}}</span>
            </template>
            <template v-slot:header.amountOfUsers="{header}">
                <span>{{header.text}}</span>
            </template>
            <template v-slot:header.createTime="{header}">
                <span>{{header.text}}</span>
            </template>
            <template v-slot:header.expiration="{header}">
                <span>{{header.text}}</span>
            </template>


            <template v-slot:item.couponType="{value}">
                {{value.toLowerCase() === 'flat' ? $t('marketing_coupon_type_flat') : $t('marketing_coupon_type_percentage')}}
            </template>
            <template v-slot:item.createTime="{value}">
                {{$moment(value).format('MMM Do, YYYY')}}
                <!-- {{$d(new Date(value), 'tableDate')}} -->
            </template>
            <template v-slot:item.expiration="{value}">
                {{$moment(value).format('MMM Do, YYYY')}}
            </template>
            <template v-slot:no-data>
                {{$t('marketing_tableCoupon_noCoupons')}}
            </template>
        </v-data-table>
    </div>
</template>

<script>
import storeService from '../../../../services/store/storeService';
import couponStore from '../../../../store/couponStore';

export default {
    name: "tableCoupon",
    data() {
        return {
            tableLoading: false,
            headers:[
                {
                    text: this.$t('marketing_tableCoupon_code'),
                    align: '',
                    value: 'code',
                },
                {
                    text: this.$t('marketing_tableCoupon_type'),
                    align: '',
                    value: 'couponType',
                },
                {
                    text: this.$t('marketing_tableCoupon_value'),
                    align: '',
                    value: 'value',
                },
                {
                    text: this.$t('marketing_tableCoupon_amount'),
                    align: '',
                    value: 'amountOfUsers',
                },
                {
                    text: this.$t('marketing_tableCoupon_created'),
                    align: '',
                    value: 'createTime',
                },
                {
                    text: this.$t('marketing_tableCoupon_expired'),
                    align: '',
                    value: 'expiration',
                },
            ],
        }
    },
    computed: {
        coupons(){
            return this.$store.getters.getCouponList
        }
    },
    methods: {
      getCoupons() {
        let self = this;
        self.tableLoading = true;
        self.$store.dispatch('getUserCoupons').catch(ex => {
          self.$appInsights.trackException({exception: new Error(ex)});
        }).finally(() => {
            self.tableLoading = false;
        })
      }
    },
    beforeDestroy(){
      storeService.unregisterModule(this.$store, 'couponStore');
    },
    created() {
      storeService.registerModule(this.$store, 'couponStore', couponStore);
      this.getCoupons();
    }
}
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    @import '../../../../styles/colors.less';
    .tableCoupon  {
        background: #fff;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
        border-radius: 8px;
        max-width: 1080px;
        // max-width: 1366px;
        margin: 24px 34px;
        @media (max-width: @screen-xs) {
            box-shadow: none;
            border-radius: 0;
        }
        .myCoupons_title {
            font-weight: 600;
            font-size: 16px;
            color: @global-purple;
        }
        .dataTable {
            width: 100%;
            .v-data-table-header {
                th {
                    font-weight: normal;
                    color: @global-purple;
                    font-size: 14px;
                }

            }
            td {
                font-size: 14px;
            }
            .v-data-footer {
                .v-data-footer__select,
                .v-data-footer__pagination {
                    color: @global-purple;
                    font-size: 14px;
                    opacity: .6;
                }
                .v-data-footer__icons-before,
                .v-data-footer__icons-after {
                    .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
                        font-size: 14px;
                    }
                }
            }
            .tableTop {
                padding: 20px;
                color: @global-purple !important;
                .myCoupons_title {
                    font-size: 22px;
                    font-weight: 600;
                    line-height: 1.3px;
                    @media (max-width: @screen-xs) {
                    line-height: initial;
                    }
                    background: #fff;
                }

                .couponBtn {
                    text-transform: initial !important;
                }
            }
        }
    }
</style>