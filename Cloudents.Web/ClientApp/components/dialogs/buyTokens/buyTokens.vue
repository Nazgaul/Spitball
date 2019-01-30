<template>
  <div class="buy-dialog-wrap">
    <div class="buy_title">
      <h1>select a product</h1>
    </div>
    <div class="buy_content">
      <div
        @click="selectProduct('basic')"
        :class="{'item-selected': selectedProduct === 'basic'}"
      >basic</div>
      <div
        @click="selectProduct('inter')"
        :class="{'item-selected': selectedProduct === 'inter'}"
      >inter</div>
      <div @click="selectProduct('pro')" :class="{'item-selected': selectedProduct === 'pro'}">pro</div>
    </div>

    <div class="buy_button_container">
      <div class="paypal-button" id="paypal-button"></div>
    </div>
  </div>
</template>

<script>
import {mapGetters} from 'vuex';
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
          currency: "USD"
        },
        inter: {
          name: "Tier 2 SBL pack",
          description: "you will recieve 200 sbl",
          price: "2",
          quantity: "1",
          currency: "USD"
        },
        pro: {
          name: "Tier 3 SBL pack",
          description: "you will recieve 300 sbl",
          price: "3",
          quantity: "1",
          currency: "USD"
        }
      },
      paypalLoaded: false,
      user: this.accountUser()
    };
  },
  methods: {
    ...mapGetters(['accountUser']),
    selectProduct(val) {
      if (this.selectedProduct !== val) {
        this.selectedProduct = val;
        let paypalBtn = document.getElementById("paypal-button");
        paypalBtn.innerHTML = "";
        this.mountPaypalButton();
      }
    },
    reflectPaymentToServer(transactionId){
        console.log(`transaction made id is ${transactionId}`);
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
              size: "small",
              color: "blue",
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
</script>

<style lang="less">
.buy-tokens-popup {
  box-shadow: none;
  justify-content: center;
  background: #eee;
  width: 500px;
  .buy-dialog-wrap {
    width: 100%;
    .buy_title {
    }
    .buy_content {
      height: 150px;
      display: flex;
      justify-content: space-evenly;
      width: 100%;
      div {
        width: 125px;
        border: 2px solid gray;
        &.item-selected {
          border: 2px solid green;
        }
      }
    }
    .buy_button_container {
            display: flex;
            justify-content: center;
            .paypal-button{
                margin-top:15px;
            }
    }
  }
}
</style>
