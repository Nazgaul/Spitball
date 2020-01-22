<template>
    <div id="registerStudentCollege">
        <!-- <component :is="studentCollegeStep">
            <template #titleUniversity>
                <div class="text-center university_title" v-language:inner="'register_university_title'"></div>
            </template>

            <template #titleCourse>
                <div class="text-center courses_title" v-language:inner="'register_course_title'"></div>
            </template>
        </component> -->

        <router-view>
            <template #titleUniversity>
                <div class="text-center university_title" v-language:inner="'register_university_title'"></div>
            </template>

            <template #titleCourse>
                <div class="text-center courses_title" v-language:inner="'register_course_title'"></div>
            </template>
        </router-view>
    </div>
</template>

<script>
const registerUniversity = () => import('../registerUniversity/registerUniversity.vue')
const registerCourse = () => import('../registerCourse/registerCourse.vue')

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