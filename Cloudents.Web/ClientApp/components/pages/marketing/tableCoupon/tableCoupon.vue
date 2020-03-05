<template>
    <v-row class="tableCoupon fullWidth mt-2 mt-sm-4 pa-4 pb-2 pb-sm-0 ma-0">
        <v-col cols="12" class="pa-0">
            <div class="mainTitle mb-2">{{$t('marketing_tableCoupon_title')}}</div>
        </v-col>
        <v-data-table
            :headers="headers"
            :items="coupons"
            class="dataTable"
            :loading="tableLoading"
            :mobile-breakpoint="0"
            :footer-props="{
                showFirstLastPage: false,
                prevIcon: 'sbf-arrow-left-carousel',
                nextIcon: 'sbf-arrow-right-carousel',
                itemsPerPageOptions: [5]
            }">

            <template v-slot:header.code="{header}">
                {{$t(header.text)}}
            </template>
            <template v-slot:header.couponType="{header}">
                {{$t(header.text)}}
            </template>
            <template v-slot:header.value="{header}">
                {{$t(header.text)}}
            </template>
            <template v-slot:header.amountOfUsers="{header}">
                {{$t(header.text)}}
            </template>
            <template v-slot:header.createTime="{header}">
                {{$t(header.text)}}
            </template>
            <template v-slot:header.expiration="{header}">
                {{$t(header.text)}}
            </template>


            <template v-slot:item.couponType="{value}">
                {{$t(value === 'Flat' ? 'marketing_coupon_type_flat' : 'marketing_coupon_type_percentage')}}
            </template>
            <template v-slot:item.createTime="{value}">
                {{$d(new Date(value), 'tableDate')}}
            </template>
            <template v-slot:item.expiration="{value}">
                {{value ? $d(new Date(value), 'tableDate') : ''}}
            </template>
            <template v-slot:no-data>
                {{$t('marketing_tableCoupon_noCoupons')}}
            </template>

            <template v-slot:no-data>
                {{$t('marketing_tableCoupon_noCoupons')}}
            </template>
        </v-data-table>
    </v-row>
</template>

<script>
// import storeService from '../../../../services/store/storeService';
// import couponStore from '../../../../store/couponStore';

export default {
    name: "tableCoupon",
    data: () => ({
        tableLoading: false,
        coupons: [],
        headers:[
            {
                text: 'marketing_tableCoupon_code',
                align: 'left',
                value: 'code',
            },
            {
                text: 'marketing_tableCoupon_type',
                align: 'left',
                value: 'couponType',
            },
            {
                text: 'marketing_tableCoupon_value',
                align: 'left',
                value: 'value',
            },
            {
                text: 'marketing_tableCoupon_amount',
                align: 'left',
                value: 'amountOfUsers',
            },
            {
                text: 'marketing_tableCoupon_created',
                align: 'left',
                value: 'createTime',
            },
            {
                text: 'marketing_tableCoupon_expired',
                align: 'left',
                value: 'expiration',
            },
        ],
    }),
    methods: {
      getCoupons() {
        let self = this;
        self.tableLoading = true;
        self.$store.dispatch('getUserCoupons').then(coupons => {
          self.coupons = coupons;
        }).catch(ex => {
          self.$appInsights.trackException({exception: new Error(ex)});
        }).finally(() => {
            self.tableLoading = false;
        })
      }
    },
    // beforeDestroy(){
    //   storeService.unregisterModule(this.$store, 'couponStore');
    // },
    created() {
    //   storeService.registerModule(this.$store, 'couponStore', couponStore);
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
    
        @media (max-width: @screen-xs) {
            box-shadow: none;
            border-radius: 0;
        }
        .mainTitle {
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
                        transform: none /*rtl:rotate(180deg)*/;
                        color: @global-purple !important; //vuetify
                        font-size: 14px;
                    }
                }
            }
        }
    }
</style>