<template>
    <div id="registerStudentSchool">
        <router-view>
            <template #titleCourse>
                <div class="text-center maintitle" v-language:inner="'register_school_title'"></div>
                <div class="text-center subtitle" v-language:inner="'register_school_subtitle'"></div>
                <div class="gradesWrap">
                    <v-select
                        v-model="grades"
                        :items="items"
                        class="gradesWrap_select mb-2"
                        outlined
                        dense
                        height="44"
                        :menu-props="{ maxHeight: '400' }"
                        label="What grade you are"
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
export default {
    data: () => ({
        items: [1, 2, 3, 4],
        showGradeError: false
    }),
    computed: {
        grades: {
            get() {
                return this.$store.getters.getStudentGrade;
            },
            set(grade) {
                this.setGrade(grade)
            }
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