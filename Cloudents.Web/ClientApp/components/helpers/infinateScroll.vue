<template>
        <div v-scroll="{callback:this.scrollList}"><slot></slot></div>
</template>
<script>

    export default {
        data() {
            return {
                page: this.token||1,
                currentToken:null,
                isLoading:false,
                isComplete:false
            }
        },

        props: { token: { type: [String, Number]}},

        created(){
            this.currentToken=this.token;
        },
        watch:{
            '$route':'$_resetScrolling'
        },

        methods: {
            keepLoad(){
                let totalHeight=document.body.scrollHeight;
                let currentScroll=window.scrollY;
                let scrollOffset=(currentScroll>0.75*totalHeight);
                let retVal=((window.pageYOffset>0 || document.documentElement.scrollTop>0)&&scrollOffset&&
                    !this.isLoading&&!this.isComplete);
                return retVal},
            scrollList () {
                if (this.keepLoad()) {
                    let page=this.token?this.currentToken:this.page;
                    this.isLoading=true;
                    this.$store.dispatch('fetchingData', { name: this.$route.path.slice(1), params: this.$route.query, page })
                        .then(({data:res}) => {
                            if (res.data && res.data.length && !res.hasOwnProperty('token') ||
                                (res.hasOwnProperty('token') && res.token)) {
                                this.$emit('scroll', res.data);
                                if (res.hasOwnProperty('token')) {
                                    this.currentToken = res.token;
                                } else { this.page++; }
                                this.isLoading=false;
                            } else {
                                this.page = 1;
                                this.isComplete=true;
                            }
                        }).catch(reason => {
                        this.isComplete=true;
                    })
                }
            },
            $_resetScrolling(){
                    this.page=this.token?this.token:1;
                    this.isLoading=false;
                    this.isComplete=false;
            }

        }
    }
</script>