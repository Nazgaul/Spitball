<template>
    <div id="registerTeacher">
        <!-- <component :is="teacherStep"></component> -->

        <button @click="openBecomeTutorDialog">click</button>
    </div>
</template>

<script>
export default {
    name: 'registerTeacher',
    data: () => ({
        
    }),
    methods: {
        openBecomeTutorDialog() {
            debugger
            this.$store.dispatch('updateTutorDialog',true)
        }
    }
}
</script>

<style lang="less">

</style>