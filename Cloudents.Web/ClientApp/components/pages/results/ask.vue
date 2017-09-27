<template>
    <div>
        <router-link :to="{name:$route.name,query:{a:'yiaft'}}">yifatttttt</router-link>

        <div>
            ask
        </div>
        <div v-if="content">
            <item-result :item="item" v-for="item in items"></item-result>
        </div>
        <div v-else>There is no data</div>
    </div>
</template>
<script>
    import itemResult from './itemResult.vue'
    export default {
        computed: {
            content: function () { return this.$store.getters.pageContent },
            items: function (){return this.$store.getters.pageContent.items
        } ,
            answer: function () { return this.$store.getters.pageContent.shortAnswer }
        }, components: {itemResult}
    }
</script>
