<template>
    <div id="registerParent">

        <div class="text-center mainTitle" v-language:inner="'register_school_title'"></div>
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
const registerCourse = () => import('../registerCourse/registerCourse.vue')

export default {
    // name: 'registerParent',
    components: {
        registerCourse
    },
    data: () => ({
        firstname: '',
        lastname: '',
        grade: null,
        grades: [1, 2, 3, 4]
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

<style lang="less">
@color: #43425d;

#registerParent {
    max-width: 500px;
    .mainTitle {
        font-size: 26px;
        color: @color;
        margin-bottom: 10px;
    }
    .subtitle {
        font-size: 16px;
        color: @color;
        margin-bottom: 40px;
    }

    .gradesWrap_select {
        i {
            font-size: 8px;
            color: @color;
        }
    }
}

</style>