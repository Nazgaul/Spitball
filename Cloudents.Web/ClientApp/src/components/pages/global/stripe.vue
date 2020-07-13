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
    async mounted() {
        await this.$loadScript("https://js.stripe.com/v3/?advancedFraudSignals=false");
        this.stripe =  await window.Stripe(window.stripe);

    }
}
</script>

