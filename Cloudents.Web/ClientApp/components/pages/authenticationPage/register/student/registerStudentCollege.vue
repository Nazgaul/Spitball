<template>
    <div id="registerStudentCollege">
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

export default {
    // name: 'registerStudentCollege',
}
</script>

<style lang="less">

#registerStudentCollege {

}

</style>