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
                        class="gradesWrap_select mb-2"
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
            <v-btn cols="12" type="submit">send</v-btn>
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
        firstname: '',
        lastname: '',
        grade: null,
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
        label: {
            fname: LanguageService.getValueByKey('register_student_parent_fname'),
            lname: LanguageService.getValueByKey('register_student_parent_lname'),
            grade: LanguageService.getValueByKey('register_student_parent_grade'),
        }
    }),
    methods: {
        send() {
            let parentObj = {
                firstname: this.firstname,
                lastname: this.lastname,
                grade: this.grade
            }
            this.$store.dispatch('parentRegister', parentObj)
        }
    }
}
</script>
