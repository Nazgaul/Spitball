<template>
    <div id="registerStudentSchool">
        
        <component :is="'registerCourse'">

            <template #titleCourse>
                <div class="text-center mainTitle" v-language:inner="'register_school_title'"></div>
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
            
        </component>
    </div>
</template>

<script>
const registerUniversity = () => import('../registerUniversity/registerUniversity.vue')
const registerCourse = () => import('../registerCourse/registerCourse.vue')

export default {
    name: 'registerStudentSchool',
    components: {
        registerUniversity,
        registerCourse,
    },
    data: () => ({
        grades: '',
        items: ['High school1', 'High school2', 'High school3', 'High school4']
    })
}
</script>

<style lang="less">
@color: #43425d;

#registerStudentSchool {

    .mainTitle {
        font-size: 26px;
        color: @color;
        margin-bottom: 10px;
    }
    .subtitle {
        font-size: 16px;
        color: @color;
        margin-bottom: 46px;
    }
    .gradesWrap {
        width: 250px;
        i {
            font-size: 8px;
            color: @color;
        }
    }
}
</style>