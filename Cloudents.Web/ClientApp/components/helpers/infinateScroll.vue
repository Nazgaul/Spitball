<template>
    <div>
        <slot></slot>
        <infinite-loading @infinite="infiniteHandler" ref="infiniteLoading">
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
                page: this.token||1,
                currentToken:null
            }
        },

        props: { loadMore: { type: Boolean }, token: { type: [String, Number]}},

        created(){
            this.currentToken=this.token;
        },
        watch:{
            '$route':'$_resetScrolling'
        },
        methods: {
            $_resetScrolling(){
                    this.page=this.token?this.token:1;
                    if(this.token){this.currentToken=this.token;this.page=1;}
                    this.currentQuery=this.$route.fullPath;
                    this.$refs.infiniteLoading.isComplete=false;
                    this.$emit('$InfiniteLoading:reset');
            },
            infiniteHandler($state) {

              if (this.loadMore) {
                    let page=this.token?this.currentToken:this.page;
                    this.$store.dispatch('scrollingItems', { name: this.$route.path.slice(1), params: this.$route.query, page })
                        .then((res) => {
                            if (res.data && res.data.length && !res.hasOwnProperty('token') ||
                                (res.hasOwnProperty('token') && res.token)) {
                                this.$emit('scroll', res.data);
                                $state.loaded();
                                if (res.hasOwnProperty('token')) {
                                    this.currentToken = res.token;
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