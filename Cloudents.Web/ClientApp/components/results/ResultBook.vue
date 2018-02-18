<template v-once>
    <router-link class="pa-2 d-block" :to="{name:'bookDetails',params:{id:item.isbn13}}">
        <span @click="$ga.event('Search_Results', 'Textbooks',`#${index+1}_click`)">
            <book-cell :item="item"></book-cell>
        </span>
    </router-link>
</template>
<script>
    import bookCell from "../book/bookCell.vue"
    export default {
        components: { bookCell },
        props: { item: { type: Object, required: true }, isDetails: { type: Boolean, default: false }, index: { Number } }
    }
</script>
