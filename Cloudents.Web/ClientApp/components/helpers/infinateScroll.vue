<template>
    <div>
        <slot></slot>
        <infinite-loading @infinite="infiniteHandler">
            <span slot="no-more">
            </span>
            <div slot="no-results"></div>
            <div slot="no-more"></div>
        </infinite-loading>
    </div>
</template>
<script>
    import InfiniteLoading from 'vue-infinite-loading';

    export default {
        data() {
            return {
                page: 1
            }
        },

        props: { loadMore: {type:Boolean} },

        methods: {
            infiniteHandler($state) {
              if (this.loadMore) {
                    this.$store.dispatch('scrollingItems', { name: this.$route.name, params: this.$router.query, page: this.page })
                        .then(({ data }) => {
                            if (data && data.length) {
                                this.$emit('scroll', data);
                                $state.loaded();
                                this.page++;
                            } else {
                                $state.complete();
                            }
                        })
                } else { $state.complete(); }
            },
        },
        components: {
            InfiniteLoading
        }
    }
</script>