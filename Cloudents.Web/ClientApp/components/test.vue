<template>
  <div>
    Hello World
    <div id="paypal-button-container"></div>
  </div>
</template>
<script>
export default {
  mounted() {
    this.$loadScript(
      `https://www.paypal.com/sdk/js?client-id=${window.paypalClientId}&commit=false`
    ).then(() => {
      window.paypal
        .Buttons({
          createOrder: function(data, actions) {
            // Set up the transaction
            return actions.order.create({
              purchase_units: [
                {
                  reference_id: "PUHF",
                  amount: {
                    value: "100",
                    currency_code: "USD"
                  }
                }
              ]
            });
          },
          onApprove: function(data, actions) {
              debugger;
               const response = fetch("api/wallet", {
                    method: 'POST', // *GET, POST, PUT, DELETE, etc.
   
                    headers: {
                    'Content-Type': 'application/json'
                    // 'Content-Type': 'application/x-www-form-urlencoded',
                    },
                    redirect: 'follow', // manual, *follow, error
                    body: JSON.stringify({orderId : data.orderID}) // body data type must match "Content-Type" header
                });
                response.then(() => {
                    debugger;
                });

                  
            }
        })
        .render("#paypal-button-container");

      //             paypal.Buttons({
      //     createOrder: function(data, actions) {
      //       // Set up the transaction
      //       return actions.order.create({
      //         purchase_units: [{
      //           amount: {
      //             value: '0.01'
      //           }
      //         }]
      //       });
      //     }
      //   }).render('#paypal-button-container');
    });
  }
};
</script>
