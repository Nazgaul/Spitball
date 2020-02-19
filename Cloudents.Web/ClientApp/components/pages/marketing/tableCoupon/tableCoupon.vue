<template>
    <v-row class="tableCoupon mt-4 pa-4 pb-2 pb-sm-0" dense>
        <v-col cols="12" class="pa-0">
            <div class="mainTitle">{{$t('tableCoupon_title')}}</div>
        </v-col>
        <v-data-table
            :headers="headers"
            :items="coupons"
            class="dataTable"
            sort-by
            :loading="tableLoading"
            :mobile-breakpoint="0"
            :footer-props="{
              showFirstLastPage: false,
              prevIcon: 'sbf-arrow-left-carousel',
              nextIcon: 'sbf-arrow-right-carousel',
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
            <template v-slot:header.amountUsers="{header}">
                {{$t(header.text)}}
            </template>
            <template v-slot:header.createTime="{header}">
                {{$t(header.text)}}
            </template>
            <template v-slot:header.expiration="{header}">
                {{$t(header.text)}}
            </template>              
                
            <template v-slot:item.createTime="{value}">
                {{$d(new Date(value), 'tableDate')}}
            </template>

            <template v-slot:item.expiration="{value}">
                {{$d(new Date(value), 'tableDate')}}
            </template>

        </v-data-table>
    </v-row>
</template>

<script>
import storeService from '../../../../services/store/storeService';
import couponStore from '../../../../store/couponStore';

export default {
    name: "tableCoupon",
    data: () => ({
        tableLoading: false,
        coupons: [],
        headers:[
            {
                // text: this.$t('tableCoupon_code'),
                text: 'tableCoupon_code',
                align: 'left',
                sortable: false,
                value: 'code',
            },
            {
                // text: this.$t('tableCoupon_type'),
                text: 'tableCoupon_type',
                align: 'left',
                sortable: false,
                value: 'couponType',
            },
            {
                // text: this.$t('tableCoupon_value'),
                text: 'tableCoupon_value',
                align: 'left',
                sortable: false,
                value: 'value',
            },
            {
                // text: this.$t('tableCoupon_amount'),
                text: 'tableCoupon_amount',
                align: 'left',
                sortable: false,
                value: 'amountUsers',
            },
            {
                // text: this.$t('tableCoupon_created'),
                text: 'tableCoupon_created',
                align: 'left',
                sortable: false,
                value: 'createTime',
            },
            {
                // text: this.$t('tableCoupon_expired'),
                text: 'tableCoupon_expired',
                align: 'left',
                sortable: false,
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
      },
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
    
        @media (max-width: @screen-xs) {
            box-shadow: none;
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

            tr {
                color: @global-purple;
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