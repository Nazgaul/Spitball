<template>
    <component v-if="query" :is="query"></component>
</template>

<script>

const becomeTutor = () => import('../../../becomeTutor/becomeTutor.vue');

export default {
    components: {
        becomeTutor
    },
    data: () => ({
        query: ''
    }),
    watch: {
        "$route.query.dialog"(query) {
            this.query = query ? query : '';
        }
    }
}
</script>

<style lang="less">

</style>