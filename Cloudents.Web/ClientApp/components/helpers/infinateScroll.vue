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
                page: this.token
            }
        },

        props: { loadMore: { type: Boolean }, token: { type: [String, Number],default:1}},

        methods: {
            infiniteHandler($state) {
              if (this.loadMore) {
                    this.$store.dispatch('scrollingItems', { name: this.$route.path.slice(1), params: this.$router.query, page: this.page })
                        .then((res) => {
                            if (res.data && res.data.length) {
                                this.$emit('scroll', res.data);
                                $state.loaded();
                                if (res.hasOwnProperty('token')) {
                                    this.page = res.token;
                                } else { this.page++; }
                                
                            } else {
                                this.page = 1;
                                $state.complete();
                            }
                        })
              } else {
                  this.page = 1;
                  $state.complete();
              }
            },
        },
        components: {
            InfiniteLoading
        }
    }
</script>