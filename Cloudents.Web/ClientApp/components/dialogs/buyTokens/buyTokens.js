import {mapGetters, mapActions} from 'vuex';
import walletService from '../../../services/walletService';
import {LanguageService} from '../../../services/language/languageService';

export default {
  data() {
    return {
      selectedProduct: null,
      products: {
        basic: {
          name: "Tier 1 SBL pack",
          description: "you will recieve 100 sbl",
          price: "1",
          quantity: "1",
          currency: "USD",
          sku: "points_1"
        },
        inter: {
          name: "Tier 2 SBL pack",
          description: "you will recieve 200 sbl",
          price: "2",
          quantity: "1",
          currency: "USD",
          sku: "points_2",
        },
        pro: {
          name: "Tier 3 SBL pack",
          description: "you will recieve 300 sbl",
          price: "3",
          quantity: "1",
          currency: "USD",
          sku: "points_3"
        }
      },
      paypalLoaded: false,
      user: this.accountUser()
    };
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
            toasterText: LanguageService.getValueByKey("buyToken_success"),
            showToaster: true,
          });
        }, (error)=>{
          //fallback will be called on app.vue create method.
          global.localStorage.setItem('sb_transactionError', transactionId);
          // global.location.reload();
          console.log(error);
        })
        // window.alert("Thank you for your purchase!");
    },
    mountPaypalButton() {
      if (this.paypalLoaded) {
        paypal.Button.render(
          {
            // Configure environment
            env: "sandbox",
            client: {
              sandbox:
                "AcaET-3DaTqu01QZ0Ad7-5C52pMZ5s4nx59TmbCqdn8gZpfJoM3UPLYCnZmDELZfc-22N_yhmaGEjS3e"
              // production: 'demo_production_client_id'
            },
            // Customize button (optional)
            locale: "en_US",
            style: {
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
                      total: this.products[this.selectedProduct].price,
                      currency: this.products[this.selectedProduct].currency,
                    },
                    item_list: {
                      items: [this.products[this.selectedProduct]]
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
      }
    );
  }
};