<template>
    <div id="registerParent">

        <div class="text-center maintitle" v-language:inner="'register_school_title'"></div>
        <div class="text-center subtitle" v-language:inner="'register_parent_subtitle'"></div>

        <form @submit.prevent="send">
            <v-row>
                <v-col cols="12" sm="4" class="pb-0">
                    <v-text-field
                        v-model="firstname"
                        label="Student first name"
                        placeholder=" "
                        outlined
                        height="44"
                        dense
                    ></v-text-field>                        
                </v-col>
                <v-col cols="12" sm="4" class="pb-0">
                    <v-text-field
                        v-model="lastname"
                        label="Student Last name"
                        placeholder=" "
                        outlined
                        height="44"
                        dense
                    ></v-text-field>                    
                </v-col>
                <v-col cols="12" sm="4" class="pb-0">
                    <v-select
                        v-model="grade"
                        :items="grades"
                        class="gradesWrap mb-2"
                        outlined
                        dense
                        height="44"
                        :menu-props="{ maxHeight: '400' }"
                        label="Student grade"
                        placeholder=" "
                        append-icon="sbf-triangle-arrow-down">
                    </v-select>
                </v-col>
            </v-row>
        </form>

        <registerCourse />
    </div>
</template>

<script>
import { LanguageService } from '../../../../../services/language/languageService';

const registerCourse = () => import('../registerCourse/registerCourse.vue');

export default {
    components: {
        registerCourse
    },
    data: () => ({
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
        label: {
            fname: LanguageService.getValueByKey('register_student_parent_fname'),
            lname: LanguageService.getValueByKey('register_student_parent_lname'),
            grade: LanguageService.getValueByKey('register_student_parent_grade'),
        }
    }),

    computed: {
        firstname: {
            get() {
                return this.$store.getters.getFirstName
            },
            set(firstname) {
                this.$store.dispatch('updateFirstName', firstname)
            }
        },
        lastname: {
            get() {
                return this.$store.getters.getLastName
            },
            set(lastname) {
                this.$store.dispatch('updateLastName', lastname)
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
    }
}
</script>
