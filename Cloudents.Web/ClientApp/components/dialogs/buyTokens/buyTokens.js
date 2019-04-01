import {mapGetters, mapActions} from 'vuex';
import walletService from '../../../services/walletService';
import {LanguageService} from '../../../services/language/languageService';
import analyticsService from '../../../services/analytics.service';

export default {
  data() {
    return {
      selectedProduct: null,
      showOverlay: false,
      products:{
          currency: LanguageService.getValueByKey('buyTokens_currency'),
          basic:{
              pts:100,
              price:1.5,
              currency: 'USD'
          },
          inter:{
              pts:500,
              price:6,
              currency: 'USD'
          },
          pro:{
              pts:1400, 
              price:14,
              currency: 'USD'
          }
      },
      productsForPaypal: {
        basic: {
          name: "100 points on Spitball",
          description: "100 points on Spitball",
          quantity: "1",
          sku: "points_1"
        },
        inter: {
          name: "500 points on Spitball",
          description: "500 points on Spitball",
          quantity: "1",
          sku: "points_2",
        },
        pro: {
          name: "1400 points on Spitball",
          description: "1400 points on Spitball",
          quantity: "1",
          sku: "points_3"
        }
      },
      paypalLoaded: false,
      user: this.accountUser()
    };
  },
  computed:{
    basicConvertionRate(){
        return this.products.basic.price / this.products.basic.pts
    },
    interConvertionRate(){
        return this.products.inter.price / this.products.inter.pts
    },
    proConvertionRate(){
        return (this.products.pro.price / this.products.pro.pts).toFixed(3);
    }
  },
  methods: {
    ...mapGetters(['accountUser']),
    ...mapActions(['updateShowBuyDialog', 'updateShowBuyDialog', 'updateToasterParams']),
    selectProduct(val) {
      if (this.selectedProduct !== val) {
        this.selectedProduct = val;
        let paypalBtn = document.getElementById("paypal-button");
        paypalBtn.innerHTML = "";
        this.mountPaypalButton();
      }
    },
    closeModal(){
      this.updateShowBuyDialog(false)
    },
    reflectPaymentToServer(transactionId){
        console.log(`transaction made id is ${transactionId}`);
        let transactionObject = {
          id: transactionId
        }
        walletService.buyTokens(transactionObject).then(()=>{
          this.updateShowBuyDialog(false);
          this.updateToasterParams({
            toasterText: LanguageService.getValueByKey("buyTokens_success_transaction"),
            showToaster: true,
          });
        }, (error)=>{
          //fallback will be called on app.vue create method.
          global.localStorage.setItem('sb_transactionError', transactionId);
          this.updateToasterParams({
            toasterText: LanguageService.getValueByKey("buyTokens_failed_transaction"),
            showToaster: true,
          });
          // global.location.reload();
          console.log(error);
        }).finally(()=>{
            this.showOverlay = false;  
        })
        // window.alert("Thank you for your purchase!");
    },
    mountPaypalButton() {
      if (this.paypalLoaded) {
        analyticsService.sb_unitedEvent("BUY_POINTS", "PRODUCT_SELECTED", this.selectedProduct);
        //set price and currency according to the locale
        this.productsForPaypal[this.selectedProduct].price = this.products[this.selectedProduct].price;
        this.productsForPaypal[this.selectedProduct].currency = this.products[this.selectedProduct].currency;

        paypal.Button.render(
          {
            // Configure environment
            env: global.paypalEnv,
            client: {
              sandbox: "AcaET-3DaTqu01QZ0Ad7-5C52pMZ5s4nx59TmbCqdn8gZpfJoM3UPLYCnZmDELZfc-22N_yhmaGEjS3e",
              production: "AQ_i7yH6NyGmUeJtuVfrSwK_RSb8rydP2f5zkh5rqyF_qgq_mT_gakcFZUmgY7HF-6YvneG4xQlOEz4Q"
            },
            // Customize button (optional)
            locale: "en_US",
            style: {
              // layout: 'vertical',
              size: "medium",
              color: "gold",
              shape: "pill",
              tagline: false
            },

            // Enable Pay Now checkout flow (optional)
            commit: true,

            // Set up a payment
            payment: (data, actions) => {
              return actions.payment.create({
                payer: {
                    payment_method: "paypal",                    
                },
                transactions: [
                  {
                    amount: {
                      total: this.productsForPaypal[this.selectedProduct].price,
                      currency: this.productsForPaypal[this.selectedProduct].currency,
                    },
                    item_list: {
                      items: [this.productsForPaypal[this.selectedProduct]]
                    },
                  }
                ]
              });
            },
            // Execute the payment
            onAuthorize: (data, actions)=> {
              return actions.payment.execute().then((response)=> {
                // Show a confirmation message to the buyer
                let transactionId = response.id;
                this.showOverlay = true;
                this.reflectPaymentToServer(transactionId);
              });
            }
          },
          "#paypal-button"
        );
      } else {
        console.log("error loading paypal");
      }
    }
  },
  created() {
    this.$loadScript("https://www.paypalobjects.com/api/checkout.js").then(
      () => {
        this.paypalLoaded = true;
          this.selectProduct('inter')
      }
    );
  }
};