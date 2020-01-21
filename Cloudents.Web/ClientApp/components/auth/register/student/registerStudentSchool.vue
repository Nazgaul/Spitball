<template>
    <div id="registerStudentSchool">
        
        <component :is="'registerCourse'"></component>
    </div>
</template>

<script>
const registerUniversity = () => import('../registerUniversity.vue')
const registerCourse = () => import('../registerCourse.vue')

export default {
    name: 'registerStudentSchool',
    components: {
        registerUniversity,
        registerCourse   
    }
}
</script>

<style lang="less">

#registerStudentSchool {

}

</style>