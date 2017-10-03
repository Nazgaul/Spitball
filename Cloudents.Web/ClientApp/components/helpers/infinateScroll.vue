<template>
    <div>
       <slot></slot>
        <infinite-loading @infinite="infiniteHandler">
            <!--<span slot="no-more">
                There is no more Hacker News
            </span>-->
        </infinite-loading>
    </div>
</template>
<script>
    import InfiniteLoading from 'vue-infinite-loading';

    export default {
        data() {
            return {
                loadMore: true
            }
        },
        methods: {
            infiniteHandler($state) {
                if (this.loadMore) {
                    this.loadMore=this.$store.dispatch('scrollingItems', { name: this.$route.name, scrollState: $state});
                } else { $state.complete();}
            },
        },
        components: {
            InfiniteLoading
        }       
    }
</script>