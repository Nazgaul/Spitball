<template>
    <div id="registerStudentSchool">
        <router-view>
            <template #titleCourse>
                <div class="text-center maintitle" v-language:inner="'register_school_title'"></div>
                <div class="text-center subtitle" v-language:inner="'register_school_subtitle'"></div>
                <div class="gradesWrap">
                    <v-select
                        v-model="grade"
                        :items="grades"
                        class="gradesWrap_select mb-2"
                        outlined
                        dense
                        height="44"
                        :menu-props="{ maxHeight: '400' }"
                        :label="label"
                        placeholder=" "
                        append-icon="sbf-triangle-arrow-down"
                        hide-details>
                    </v-select>
                </div>
            </template>
        </router-view>
    </div>
</template>

<script>
import { LanguageService } from '../../../../../services/language/languageService';

export default {
    data: () => ({
        grades: [
            LanguageService.getValueByKey('register_grade1'),
            LanguageService.getValueByKey('register_grade2'),
            LanguageService.getValueByKey('register_grade3'),
            LanguageService.getValueByKey('register_grade4'),
            LanguageService.getValueByKey('register_grade5'),
            LanguageService.getValueByKey('register_grade6'),
            LanguageService.getValueByKey('register_grade7'),
            LanguageService.getValueByKey('register_grade8'),
            LanguageService.getValueByKey('register_grade9'),
            LanguageService.getValueByKey('register_grade10'),
            LanguageService.getValueByKey('register_grade11'),
            LanguageService.getValueByKey('register_grade12')
        ],
        showGradeError: false
    }),
    computed: {
        grade: {
            get() {
                return this.$store.getters.getStudentGrade;
            },
            set(grade) {
                this.setGrade(grade)
            }
        },
        label() {
            return LanguageService.getValueByKey('register_what_grade')
        }
    },
    methods: {
        setGrade(grade) {
            this.$store.dispatch('updateStudentGrade', grade).then(() => {
                this.showGradeError = false
            }).catch(() => {
                this.showGradeError = true
            })
        }
    }
}
</script>