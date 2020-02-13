<template>
    <div id="registerParent">

        <div class="text-center maintitle">{{$t('register_school_title')}}</div>
        <div class="text-center subtitle">{{$t('register_parent_subtitle')}}</div>

        <v-form lazy-validation v-model="valid" ref="form">
            <v-row>
                <v-col cols="12" sm="6" class="pb-0">
                    <v-text-field
                        v-model="fullname"
                        :label="$t('register_student_fullname')"
                        placeholder=" "
                        outlined
                        :rules="[rules.required, rules.minimumChars]"
                        height="44"
                        dense
                    ></v-text-field>
                </v-col>
                <v-col cols="12" sm="6" class="pb-0">
                    <v-select
                        v-model="grade"
                        :items="grades"
                        class="gradesWrap mb-2"
                        :label="$t('register_student_grade')"
                        outlined
                        :rules="[rules.required]"
                        dense
                        height="44"
                        :menu-props="{ maxHeight: '400' }"
                        placeholder=" "
                        append-icon="sbf-triangle-arrow-down">
                    </v-select>
                </v-col>
            </v-row>

            <registerCourse />

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
        </v-form>

    </div>
</template>

<script>
import { LanguageService } from '../../../../../services/language/languageService';

import { validationRules } from '../../../../../services/utilities/formValidationRules'

const registerCourse = () => import('../registerCourse/registerCourse.vue');

export default {
    components: {
        registerCourse
    },
    data: () => ({
        valid: true,
        grades: [
            {
                text: LanguageService.getValueByKey('register_grade1'),
                value: 1
            },
            {
                text: LanguageService.getValueByKey('register_grade2'),
                value: 2
            },
            {
                text: LanguageService.getValueByKey('register_grade3'),
                value: 3
            },
            {
                text: LanguageService.getValueByKey('register_grade4'),
                value: 4
            },
            {
                text: LanguageService.getValueByKey('register_grade5'),
                value: 5
            },
            {
                text: LanguageService.getValueByKey('register_grade6'),
                value: 6
            },
            {
                text: LanguageService.getValueByKey('register_grade7'),
                value: 7
            },
            {
                text: LanguageService.getValueByKey('register_grade8'),
                value: 8
            },
            {
                text: LanguageService.getValueByKey('register_grade9'),
                value: 9
            },
            {
                text: LanguageService.getValueByKey('register_grade10'),
                value: 10
            },
            {
                text: LanguageService.getValueByKey('register_grade11'),
                value: 11
            },
            {
                text: LanguageService.getValueByKey('register_grade12'),
                value: 12
            }
        ],
        rules: {
            required: (value) => validationRules.required(value),
            minimumChars: (value) => validationRules.minimumChars(value, 2),
        },
        label: {
            fname: LanguageService.getValueByKey('register_student_parent_fname'),
            lname: LanguageService.getValueByKey('register_student_parent_lname'),
            grade: LanguageService.getValueByKey('register_student_parent_grade'),
        }
    }),
    computed: {
        fullname: {
            get() {
                return this.$store.getters.getStudentParentFullName;
            },
            set(fullname) {
                this.$store.dispatch('updateFullName', fullname);
            }
        },
        grade: {
            get() {
                return this.$store.getters.getStudentGrade
            },
            set(grade) {
                this.$store.dispatch('updateGrade', grade)
            }
        },
    },
    methods: {
        nextStep() {
            if(this.$refs.form.validate()) {
                let self = this
                this.$store.dispatch('updateParentStudent').then(()=>{
                    self.$router.push(self.$route.meta.nextStep)
                });
            }
        },
        prevStep() {
            this.$router.push({name: this.$route.meta.backStep})
        }
    }
}
</script>

<style lang="less">
    #registerParent {
        max-width: 100%;
    }

</style>