<template>
    <v-row class="tableCoupon mt-4 pa-4 pb-2 pb-sm-0" dense>
        <v-col cols="12" class="pa-0">
            <div>{{$t('tableCoupon_title')}}</div>
        </v-col>
        <v-data-table
            :headers="headers"
            :items="coupons"
            sort-by
            :loading="tableLoading"
            :mobile-breakpoint="0"
            :footer-props="{
              showFirstLastPage: false,
              prevIcon: 'sbf-arrow-left-carousel',
              nextIcon: 'sbf-arrow-right-carousel',
              itemsPerPageText: $t('tableCoupon_rows_per_page'),
              pageText: `{1} ${$t('tableCoupon_of')} {2}`,
              itemsPerPageOptions: [5,10,15,[$t('marketing_all')]]
            }"
            class="dataTable">

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
    
        @media (max-width: @screen-xs) {
            box-shadow: none;
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
                    i {
                        font-size: 18px;
                        color: @global-purple !important; //vuetify
                    }
                }
            }
        }
    }
</style>