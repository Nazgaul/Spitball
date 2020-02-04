<template>
    <div id="registerStudentCollege">
        <router-view>
            <template #titleUniversity>
                <div class="text-center university_title">{{$t('register_university_title')}}</div>
            </template>

            <template #titleCourse>
                <div class="text-center courses_title">{{$t('register_course_title')}}</div>
            </template>
        </router-view>
        <div id="registerButtons">
            <div class="actions text-center mt-10">
                <v-btn @click="prevStep" class="btn register_btn_back" color="#4452fc" depressed height="40" outlined rounded>
                    <span>{{$t('tutorRequest_back')}}</span>
                </v-btn>
                <v-btn @click="nextStep" class="btn register_btn_next white--text" depressed rounded height="40" color="#4452fc">
                    <span>{{$t('tutorRequest_next')}}</span>
                </v-btn>
            </div>
        </div>
    </div>
</template>

<script>
import { RegisterCourseCollege } from '../../../../../routes/routeNames'

export default {
    methods: {
        nextStep() {
            if(this.$route.name == RegisterCourseCollege) {
                global.isAuth = true;
            }
            this.$router.push({name: this.$route.meta.nextStep})
        },
        prevStep() {
            this.$router.push({name: this.$route.meta.backStep})
        }
    },
}
</script>

<style lang="less">
    @import '../../../../../styles/mixin.less';

    #registerStudentCollege {
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        @media (max-width: @screen-xs) {
            height: 100%;   
        }
    }
</style>