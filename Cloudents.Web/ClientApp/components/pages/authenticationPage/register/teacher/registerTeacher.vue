<template>
    <div id="registerTeacher"></div>
</template>

<script>
export default {
    name: 'registerTeacher',
    data: () => ({
        
    }),
    methods: {
        openBecomeTutorDialog() {
            this.$store.dispatch('updateTutorDialog',true)
        }
    },
    mounted() {
        this.openBecomeTutorDialog();
    }
}
</script>

<style lang="less">

</style>