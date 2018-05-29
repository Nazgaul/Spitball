<template>
    <div v-scroll="{callback:this.scrollList}"><slot></slot></div>
</template>
<script>

    export default {
        data() {
            return {
                nextPage: null,
                isLoading: false,
                isComplete: false
            }
        },

        props: { url: String,vertical:String },

        created() {
            this.nextPage = this.url;
        },
        watch: {
            '$route': '$_resetScrolling',
            url(val){
                this.nextPage=val;
            }
        },

        methods: {
            keepLoad() {
                let totalHeight = this.$el.offsetHeight;
                let currentScroll = window.pageYOffset || document.documentElement.scrollTop;
                let scrollOffset = (currentScroll > (0.75 * totalHeight - document.documentElement.clientHeight));
                let retVal = ((window.pageYOffset > 0 || document.documentElement.scrollTop > 0) && scrollOffset &&
                    !this.isLoading && !this.isComplete);
                return retVal
            },
            scrollList() {
                if (this.keepLoad()) {
                    //debugger;
                    this.isLoading = true;
                    this.$store.dispatch('nextPage', { vertical: this.vertical, url: this.nextPage })
                        .then(({ data: res }) => {
                            if (res.data && res.data.length) {
                                this.$emit('scroll', res.data);
                                this.nextPage=res.nextPage;
                                this.isLoading = false;
                            } else {
                                this.nextPage = null;
                                this.isComplete = true;
                            }
                        }).catch(reason => {
                            this.isComplete = true;
                            this.nextPage = null;
                        })
                }
            },
            $_resetScrolling() {
                this.nextPage = null;
                this.isLoading = false;
                this.isComplete = false;
            }

        }
    }
</script>