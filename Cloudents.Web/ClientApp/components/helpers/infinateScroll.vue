<template>
    <div v-scroll="this.scrollList">
        <slot></slot>
    </div>
</template>
<script>

    export default {
        props: {
            scrollFunc: {
                type:Function,
                required: true,
                default:false
            },
            isLoading: {
                type:Boolean,
                required: true,
                default:false
            },
            isComplete: {
                type:Boolean,
                required: true,
                default:false
            }
        },
        methods: {
            keepLoad() {
                let totalHeight = this.$el.offsetHeight;
                let currentScroll = window.pageYOffset || document.documentElement.scrollTop;
                let scrollOffset = (currentScroll > (0.75 * totalHeight - document.documentElement.clientHeight));
                let retVal = (!this.isLoading && !this.isComplete && (window.pageYOffset > 0 || document.documentElement.scrollTop > 0) && scrollOffset);
                return retVal
            },
            scrollList() {
                if (this.keepLoad()) {
                    this.scrollFunc()
                }
            },
        }
    }
</script>