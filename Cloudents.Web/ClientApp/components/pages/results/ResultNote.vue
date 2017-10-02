<template>
        <item-result :items="items" ></item-result>
</template>
<script>
    import itemResult from './ResultItem.vue'
    export default {
        computed: {
            items: function () { return this.$store.getters.items } ,
            sources: function () {return this.$store.getters.pageContent.sources}
        }, components: {itemResult}
    }
</script>