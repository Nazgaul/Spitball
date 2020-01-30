<template>
    <div id="registerButtons">
        <div class="actions text-center mt-10">
            <v-btn @click="prevStep" class="btn register_btn_back" color="#4452fc" depressed height="40" outlined rounded>
                <span v-language:inner="'tutorRequest_back'"></span>
            </v-btn>
            <v-btn @click="nextStep" class="btn register_btn_next white--text" depressed rounded height="40" color="#4452fc">
                <span v-language:inner="'tutorRequest_next'"></span>
            </v-btn>
        </div>
    </div>
</template>

<script>
export default {
    computed: {
        isValid() {
            return this.$store.getters.getStepValidation
        },
        isParentRegistration() {
            return this.$route.name === 'registerCourseParent'
        },
        isHighSchoolRegistration() {
            return this.$route.name === 'registerCourse'
        }
    },
    methods: {
        nextStep() {
            if(this.isParentRegistration) {
                this.$store.dispatch('parentRegister');
            } else if(this.isHighSchoolRegistration) {
                this.$store.dispatch('updateStudentGrade')
            } else {
                if(this.isValid) {
                    this.$router.push({name: this.$route.meta.nextStep})
                    this.$store.dispatch('updateStepValidation', false);
                }
            }
        },
        prevStep() {
            this.$router.push({name: this.$route.meta.backStep})
        }
    }
}
</script>

<style lang="less">

@import '../../../../../styles/mixin.less';

#registerButtons {
    .actions {
        .btn {
            min-width: 140px;
            @media(max-width: @screen-xs) {
                min-width: 120px;
            }
        }
        .register_btn_back {
            margin-right: 10px;
        }
    }
}
</style>