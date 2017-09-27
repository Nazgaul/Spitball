<template>
    <div>
        <item-result :item="item" v-for="item in items" :key="item.id"></item-result>
    </div>
</template>
<script>
    import itemResult from './itemResult.vue'
    export default {
        computed: {
            items: function () { return this.$store.getters.items } ,
            sources: function () {return this.$store.getters.pageContent.sources}
        }, components: {itemResult}
    }
</script>