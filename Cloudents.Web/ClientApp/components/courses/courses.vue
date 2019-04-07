<template>
    <v-card class="courses-card" :class="{'': $vuetify.breakpoint.xsOnly}">
        <!--Main courses view-->
        <router-view></router-view>
        <!--Dialog with new cousre creation-->
        <sb-dialog :isPersistent="true"
                   :showDialog="createDialogVisibility"
                   :popUpType="'create-course'"
                   :max-width="'706px'"
                   :content-class="'create-course-dialog'">
            <verifyCreation v-if="!creationVerified"></verifyCreation>
            <createCourse v-if="creationVerified && !createdNew"></createCourse>
            <courseCreated v-if="createdNew" :courseName="freshCourse"></courseCreated>
        </sb-dialog>
    </v-card>
</template>

<script>
    import { mapActions, mapGetters } from "vuex";
    import sbDialog from "../wrappers/sb-dialog/sb-dialog.vue";
    import verifyCreation from './createCourses/verifyCreation.vue';
    import createCourse from './createCourses/createCourse.vue';
    import courseCreated from './createCourses/courseCreated.vue';

    export default {
        components: {
            sbDialog,
            verifyCreation,
            createCourse,
            courseCreated

        },
        data() {
            return {
                freshCourse: '',
                createdNew: false
            };
        },
        computed: {
            ...mapGetters(["createDialogVisibility", "creationVerified"]),
        },
        methods: {
            ...mapActions(["updateClasses", "updateVerification"]),
        },
        created() {
            this.$root.$on("courseCreated", courseName => {
                this.freshCourse = courseName;
                this.createdNew = true;
            });
            this.$root.$on("courseDialogClosed", val => {
                if(val){
                    this.freshCourse = '';
                    this.createdNew = false;
                    this.updateVerification(false);
                }
            });
        }
    };
</script>


<style lang="less" src="./courses.less">

</style>