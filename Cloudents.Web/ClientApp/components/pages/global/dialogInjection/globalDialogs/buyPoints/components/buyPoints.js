import {mapGetters, mapActions} from 'vuex';
import analyticsService from '../../../../../../../services/analytics.service';

export default {
  name:'buyPointsIL',
  data() {
    return {
      selectedProduct: 'inter',
      showOverlay: false,
      transactionId: 750,
      products:{
        currency: '₪',
        basic:{
            pts: 250,
            price: 10,
            currency: 'ILS'
        },
        inter:{
            pts: 750,
            price: 30,
            currency: 'ILS'
        },
        pro:{
            pts: 1500,
            price: 60,
            currency: 'ILS'
        }
      },
      user: this.accountUser()
    };
  },
  computed:{
    basicConversionRate(){
        return this.products.basic.price / this.products.basic.pts;
    },
    interConversionRate(){
        return this.products.inter.price / this.products.inter.pts;
    },
    proConversionRate(){
        return (this.products.pro.price / this.products.pro.pts).toFixed(2);
    }
  },
  methods: {
    ...mapGetters(['accountUser']),
    ...mapActions(['updateToasterParams', 'buyToken']),

    selectProduct(val) {
      if (this.selectedProduct !== val) {
        this.selectedProduct = val;
        this.transactionId = this.products[val].pts;
      }
    },

    openPaymeDialog() {
      let transactionId = this.transactionId;
      analyticsService.sb_unitedEvent("BUY_POINTS", "PRODUCT_SELECTED", transactionId);
        this.buyToken({points : transactionId});
        this.$closeDialog()
    }
  }
};