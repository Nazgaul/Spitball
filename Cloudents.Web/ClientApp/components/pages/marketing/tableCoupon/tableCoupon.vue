<template>
    <v-row class="tableCoupon mt-4" dense>
        <v-data-table
            :headers="headers"
            :items="coupons"
            sort-by
            :item-key="'date'"
            :footer-props="{
              showFirstLastPage: false,
              firstIcon: '',
              lastIcon: '',
              prevIcon: 'sbf-arrow-left-carousel',
              nextIcon: 'sbf-arrow-right-carousel',
              itemsPerPageOptions: [5],
              itemsPerPageText: 'dasdsa',
              pageText: 'ddsad'
            }"
            class="elevation-1 dataTable">
        </v-data-table>
    </v-row>
</template>

<script>
import storeService from '../../../../services/store/storeService';
import couponStore from '../../../../store/couponStore';

export default {
    name: "tableCoupon",
    data: () => ({
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
                value: 'tyoe',
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
                value: 'amount',
            },
            {
                // text: this.$t('tableCoupon_created'),
                text: 'tableCoupon_created',
                align: 'left',
                sortable: false,
                value: 'created',
            },
            {
                // text: this.$t('tableCoupon_expired'),
                text: 'tableCoupon_expired',
                align: 'left',
                sortable: false,
                value: 'expired',
            },
        ],
        coupons: [],
    }),
    methods: {
      getCoupons() {
        let self = this;
        self.$store.dispatch('getUserCoupons').then(coupons => {
          self.coupons = coupons;
        }).catch(ex => {
          self.$appInsights.trackException({exception: new Error(ex)});
        })
      }
    },
    beforeDestroy(){
      storeService.unregisterModule(this.$store, 'couponStore');
    },
    created() {
      storeService.registerModule(this.$store, 'couponStore', couponStore);
      this.getCoupons()
    }
}
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    @import '../../../../styles/colors.less';
    .tableCoupon  {
        .dataTable {
            width: 100%;
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
            border-radius: 8px;

            @media (max-width: @screen-xs) {
                box-shadow: none;
            }
        }
    }
</style>