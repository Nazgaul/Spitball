<template>
    <div class="content">
        <h1><span v-html="page.title"></span>{{dynamicHeader}}</h1>
        <slot name="options"></slot>
        <slot></slot>
        <h1 v-show="isEmpty">{{page.emptyText}}</h1><slot name="emptyState" v-show="isEmpty"></slot>
    </div>
</template>
<script>
    import { page } from './../../data'
    export default {
        data() {
            return {
                page: page[this.$route.name],
                isEmpty:this.$store.getters.isEmpty
            }
        }, computed: {
            dynamicHeader: function () { return this.$store.getters.pageTitle}
        }
    }
</script>