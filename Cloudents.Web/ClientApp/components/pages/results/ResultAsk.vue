<template>
    <div>
        <iframe v-if="video" id="video" width="420" height="315" :src="video" frameborder="0" allowfullscreen></iframe>
        <item-result :items="items"></item-result>
    </div>
</template>
<script>
    import ItemResult from './ResultItem.vue'
    import { itemsList } from './mixins'

    export default {
        mixins: [itemsList],
        computed: {
            video: function () { return this.$store.getters.pageContent.video} 
        }, components: { ItemResult}
    }
</script>
