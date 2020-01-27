<template>
    <div id="registerType">
        <div class="title text-center" v-language:inner="'loginRegister_welcome'"></div>
        <div class="subTitle text-center" v-language:inner="'loginRegister_know_better'"></div>

        <v-expansion-panels
            v-model="panel"
            class="panels d-block"
            flat
            multiple
        >
            <v-expansion-panel class="panel panel_student mb-5">
                <v-expansion-panel-header class="px-4 py-2" expand-icon="sbf-triangle-arrow-down">
                    <span class="flex-grow-0 mr-4"><studentIcon/></span>
                    <v-divider class="mr-4" vertical></v-divider>
                    <span class="panel_title text-center" v-language:inner="'loginRegister_student'"></span>
                </v-expansion-panel-header>
                <v-expansion-panel-content class="pt-4">
                    <v-btn class="mb-4 btn_student" large block color="#43425d" depressed height="50" :to="{name: 'registerCourse'}">
                        <span><collegeIcon /></span>
                        <span class="flex-grow-1 text-center white--text">A High school Student</span>
                    </v-btn>
                    <v-btn class="btn_student" large block color="#4c59ff" depressed height="50" :to="{name: 'registerUniversity'}">
                        <span><highSchoolIcon /></span>
                        <span class="flex-grow-1 text-center white--text">A College Student</span>
                    </v-btn>
                </v-expansion-panel-content>
            </v-expansion-panel>

            <router-link :to="{name: 'registerCourseParent'}">
                <v-expansion-panel class="panel panel_parent mb-5" readonly>
                    <v-expansion-panel-header class="px-4 py-2" expand-icon="">
                        <span class="flex-grow-0 mr-4"><parentIcon/></span>
                        <v-divider class="mr-4" vertical></v-divider>
                        <span class="panel_title text-center pr-5" v-language:inner="'loginRegister_parent'"></span>
                    </v-expansion-panel-header>
                </v-expansion-panel>
            </router-link>

            <router-link :to="{hash: '#becomeTutor'}">
                <v-expansion-panel class="panel panel_teacher" readonly>
                    <v-expansion-panel-header class="px-4 py-2" expand-icon="">
                        <span class="flex-grow-0 mr-4"><teacherIcon/></span>
                        <v-divider class="mr-4" vertical></v-divider>
                        <span class="panel_title text-center pr-5" v-language:inner="'loginRegister_teacher'"></span>
                    </v-expansion-panel-header>
                </v-expansion-panel>
            </router-link>
        </v-expansion-panels>

    </div>
</template>

<script>
const studentIcon = () => import('../images/student.svg');
const parentIcon = () => import('../images/parent.svg');
const teacherIcon = () => import('../images/teacher.svg');
const collegeIcon = () => import('../images/college.svg');
const highSchoolIcon = () => import('../images/highSchool.svg');

export default {
    name: '',
    components: {
        studentIcon,
        parentIcon,
        teacherIcon,
        collegeIcon,
        highSchoolIcon
    },
    data:() => ({
        panel: []
    }),
    methods: {
        fetchUser() {
            this.$store.dispatch('getUserAccountForRegister')
        }
    },
    created() {
        this.fetchUser();
    }
}
</script>

<style lang="less">
    @color: #43425d;

    #registerType {

        .title {
            font-size: 32px;
            font-weight: 600;
            color: @color;
        }
        .subTitle {
            font-size: 26px;
            color: @color;
            margin-bottom: 64px;
        }
        .panels {
            .panel {
                border-radius: 4px;
                border: solid 1px #b8c0d1;
                &::before {
                    box-shadow: none;
                }
                .panel_title {
                    font-size: 18px;
                    color: @color;
                    font-weight: 600;
                }
                &.panel_student {
                    i {
                        font-size: 8px;
                        color: @color;
                    }

                    .btn_student {
                        border-radius: 8px;
                        text-transform: initial;
                        .v-btn__content {
                            display: flex;
                            justify-content: space-between;
                        }
                    }
                }
            }
        }
    }
</style>