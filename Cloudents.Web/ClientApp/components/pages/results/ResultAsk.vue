<template>
    <div>
        <iframe v-if="video" id="video" width="420" height="315" :src="video" frameborder="0" allowfullscreen></iframe>
        <item-result :items="items"></item-result>
    </div>
</template>
<script>
    import itemResult from './ResultItem.vue'
    export default {
        computed: {
            items: function (){return this.$store.getters.items
            }, video: function () { return this.$store.getters.pageContent.video} 
        }, components: {itemResult}
    }
</script>
