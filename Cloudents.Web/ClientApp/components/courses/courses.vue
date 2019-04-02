<template>
    <v-card class="courses-card" :class="{'': $vuetify.breakpoint.xsOnly}">
        <router-view></router-view>
        <sb-dialog :isPersistent="true"
                   :showDialog="createDialogVisibility"
                   :popUpType="'create-course'"
                   :max-width="'706px'"
                   :content-class="'create-course-dialog'">
            <v-layout class="close-toolbar pl-4 pr-3" style="width:100%;" align-center justify-end
                       :class="[creationVerified ? 'dark' : 'transparent']">
                <v-flex v-show="creationVerified" grow>
                    <span class="font-weight-bold dialog-heading">Create New Course</span>
                </v-flex>
                <v-flex shrink class="mr-2">
                    <v-icon class="subheading course-close-icon"  :class="[creationVerified ? 'light' : 'dark']">sbf-close</v-icon>
                </v-flex>
            </v-layout>
            <verifyCreation v-if="!creationVerified"></verifyCreation>
            <createCourse v-else></createCourse>
        </sb-dialog>
    </v-card>
</template>

<script>
    import { mapActions, mapGetters } from "vuex";
    import  sbDialog from "../wrappers/sb-dialog/sb-dialog.vue";
    import verifyCreation from './createCourses/verifyCreation.vue';
    import createCourse from './createCourses/createCourse.vue';
    export default {
        components: {
            sbDialog,
            verifyCreation,
            createCourse

        },
        data() {
            return {
            };
        },

        computed: {
            ...mapGetters(["createDialogVisibility", "creationVerified"]),

        },
        methods: {
            ...mapActions(["updateClasses", ]),

        }
    };
</script>


<style lang="less" src="./courses.less">

</style>