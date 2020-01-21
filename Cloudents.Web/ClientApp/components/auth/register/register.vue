<template>
    <div id="registerFlow">
        <component :is="`${registerStep}`"></component>

        <template v-if="showActions">
            <div class="actions text-center mt-10">
                <v-btn @click="''" class="btn register_btn_back" color="#4452fc" depressed height="40" outlined rounded>
                    <span v-language:inner="'tutorRequest_back'"></span>
                </v-btn>
                <v-btn @click="nextStep" class="btn register_btn_next white--text" depressed rounded height="40" color="#4452fc">
                    <span v-language:inner="'tutorRequest_next'"></span>
                </v-btn>
            </div>
        </template>
    </div>
</template>

<script>
const registerType = () => import('./registerType/registerType.vue');
const registerStudentSchool = () => import('./student/registerStudentSchool.vue');
const registerStudentCollege = () => import('./student/registerStudentCollege.vue');
const registerParent = () => import('./parent/registerParent.vue');
const registerTeacher = () => import('./teacher/registerTeacher.vue');

export default {
    name: 'registerFlow',
    components: {
        registerType,
        registerStudentSchool,
        registerStudentCollege,
        registerParent,
        registerTeacher
    },
    data:() => ({
        registerStep: 'registerType'
    }),
    computed: {
        showActions() {
            return this.step !== 'registerType' && this.step !== 'registerTeacher'
        }
    }
}
</script>

<style lang="less">
#registerFlow {
    

    .actions {
        .btn {
            min-width: 140px;
        }
        .register_btn_back {
            margin-right: 10px;
        }
        .register_btn_next {

        }
    }
}
</style>