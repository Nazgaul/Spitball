<template>
  <div></div>
</template>

<script>
export default {
    data: function() {
    return {
        stripe : null
    }
    },
    methods: {
        async redirectToStripe(sessionId) {
            await this.stripe.redirectToCheckout({
                 sessionId,
            });
        }
    },
     mounted() {
        this.$loadScript("https://js.stripe.com/v3/?advancedFraudSignals=false").then(() => {
            this.stripe = window.Stripe(window.stripe);
        })
    }
}
</script>

