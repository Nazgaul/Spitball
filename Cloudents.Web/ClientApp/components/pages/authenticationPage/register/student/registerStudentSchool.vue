<template>
    <v-form id="registerStudentSchool" lazy-validation v-model="valid" action="." ref="form">
        <router-view>
            <template #titleCourse>
                <div class="text-center maintitle" v-language:inner="'register_school_title'"></div>
                <div class="text-center subtitle" v-language:inner="'register_school_subtitle'"></div>
                <div class="gradesWrap">
                    <v-select
                        v-model="grade"
                        :items="grades"
                        class="gradesWrap mb-2"
                        outlined
                        dense
                        height="44"
                        :rules="[v => v || $t('register_school_grade_error')]"
                        :menu-props="{ maxHeight: '400' }"
                        :label="label"
                        placeholder=" "
                        append-icon="sbf-triangle-arrow-down">
                    </v-select>
                </div>
            </template>
        </router-view>
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
    </v-form>
</template>

<script>
import { LanguageService } from '../../../../../services/language/languageService';

export default {
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
        showGradeError: false
    }),
    computed: {
        grade: {
            get() {
                return this.$store.getters.getStudentGrade;
            },
            set(grade) {
                this.$store.dispatch('updateGrade', grade)
            }
        },
        label() {
            return LanguageService.getValueByKey('register_what_grade')
        }
    },
    methods: {
        nextStep() {
            let form = this.$refs.form;

            if(form.validate()) {
                this.$store.dispatch('updateStudentGrade')
            }
        },
        prevStep() {
            this.$router.push({name: this.$route.meta.backStep})
        }
    },
}
</script>