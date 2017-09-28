<template> 
        <item-result :items="items" ></item-result>
</template>
<script>
    import itemResult from './itemResult.vue'
    export default {
        computed: {
            items: function () { return this.$store.getters.items}
        },
        components: {itemResult}
    }
</script>