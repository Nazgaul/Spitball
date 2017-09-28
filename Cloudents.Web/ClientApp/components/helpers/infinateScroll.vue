<template>
    <div>
       <slot></slot>
        <infinite-loading @infinite="infiniteHandler">
            <span slot="no-more">
                There is no more Hacker News
            </span>
        </infinite-loading>
    </div>
</template>
<script>
    import InfiniteLoading from 'vue-infinite-loading';
    //TO DO from const per section
    var itemsPerPage = 30;
    export default {
        computed: {
            page: function () { return (this.$store.getters.items.length / itemsPerPage)}
        },
        methods: {
            infiniteHandler($state) {
                if (Number.isInteger(this.page)) {
                    this.$store.dispatch('scrollingItems', { name: this.$route.name, page: this.page, scrollState: $state, itemsPerPage });
                } else { $state.complete();}
            },
        },
        components: {
            InfiniteLoading
        }       
    }
</script>