<template>
    <div id="registerStudentCollege">
        <component :is="'registerUniversity'">
            <template #studentCollege>
                StudentCollege
            </template>
        </component>
    </div>
</template>

<script>
const registerUniversity = () => import('../registerUniversity.vue')
const registerCourse = () => import('../registerCourse.vue')

export default {
    name: 'registerStudentCollege',
    components: {
        registerUniversity,
        registerCourse   
    }
}
</script>

<style lang="less">

#registerStudentCollege {

}

</style>