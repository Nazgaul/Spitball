<template>
    <div id="registerType">
        <div class="maintitle text-center" v-language:inner="'loginRegister_welcome'"></div>
        <div class="subtitle text-center" v-language:inner="'loginRegister_know_better'"></div>

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
                    <v-btn class="mb-4 btn_student" large block color="#43425d" depressed height="50" @click="sendRegisterType('HighSchoolStudent', {name: 'registerCourse'})">
                        <span><collegeIcon /></span>
                        <span class="flex-grow-1 text-center white--text" v-language:inner="'loginRegister_highSchool'"></span>
                    </v-btn>
                    <v-btn class="btn_student" large block color="#4c59ff" depressed height="50" @click="sendRegisterType('UniversityStudent', {name: 'registerUniversity'})">
                        <span><highSchoolIcon /></span>
                        <span class="flex-grow-1 text-center white--text" v-language:inner="'loginRegister_college'"></span>
                    </v-btn>
                </v-expansion-panel-content>
            </v-expansion-panel>

            <v-expansion-panel class="panel panel_parent mb-5" readonly @click="sendRegisterType('Parent', {name: 'registerCourseParent'})">
                <v-expansion-panel-header class="px-4 py-2" expand-icon="">
                    <span class="flex-grow-0 mr-4"><parentIcon/></span>
                    <v-divider class="mr-4" vertical></v-divider>
                    <span class="panel_title text-center pr-6" v-language:inner="'loginRegister_parent'"></span>
                </v-expansion-panel-header>
            </v-expansion-panel>

            <v-expansion-panel class="panel panel_teacher" readonly @click="sendRegisterType('Teacher', {query: {dialog: 'becomeTutor'}})">
                <v-expansion-panel-header class="px-4 py-2" expand-icon="">
                    <span class="flex-grow-0 mr-4"><teacherIcon/></span>
                    <v-divider class="mr-4" vertical></v-divider>
                    <span class="panel_title text-center pr-6" v-language:inner="'loginRegister_teacher'"></span>
                </v-expansion-panel-header>
            </v-expansion-panel>

        </v-expansion-panels>
    </div>
</template>

<script>
const studentIcon = () => import('../../images/student.svg');
const parentIcon = () => import('../../images/parent.svg');
const teacherIcon = () => import('../../images/teacher.svg');
const collegeIcon = () => import('../../images/college.svg');
const highSchoolIcon = () => import('../../images/highSchool.svg');

export default {
    components: {
        studentIcon,
        parentIcon,
        teacherIcon,
        collegeIcon,
        highSchoolIcon
    },
    data:() => ({
        panel: [],
        showError: false
    }),
    methods: {
        sendRegisterType(regType, route) {            
            this.$store.dispatch('updateRegisterType', regType).then(() => {
                this.showError = false
            }).catch(() => {
                this.showError = true
            }).finally(() => {
                this.$router.push(route)
            })
        },
    },
}
</script>

<style lang="less">
    @import '../../../../../styles/colors.less';

    #registerType {
        .panels {
            .panel {
                border-radius: 4px;
                border: solid 1px #b8c0d1;
                &::before {
                    box-shadow: none;
                }
                .panel_title {
                    font-size: 18px;
                    color: @global-purple;
                    font-weight: 600;
                }
                &.panel_student {
                    i {
                        font-size: 8px;
                        color: @global-purple;
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