<template>
    <div class="registerType text-center pa-4">

        <div class="top">
            <div>
                <div class="closeIcon text-right">
                    <v-icon size="12" color="#aaa" @click="closeRegister">sbf-close</v-icon>
                </div>
            </div>

            <div class="mainTitle" v-t="'loginRegister_registerType_mainTitle'"></div>

            <v-expansion-panels class="panels d-block px-0 px-sm-8">
                <v-expansion-panel class="panel panel_student mb-5" @click="updateStudentType">
                    <v-expansion-panel-header class="ps-0 py-0" expand-icon="">
                        <span class="panelImage flex-grow-0 px-4">
                            <studentIcon />
                        </span>
                        <v-divider class="me-4 my-2" vertical></v-divider>
                        <span class="panel_title text-center" v-t="'loginRegister_student'"></span>
                    </v-expansion-panel-header>
                </v-expansion-panel>

                <v-expansion-panel class="panel panel_teacher">
                    <v-expansion-panel-header class="ps-0 py-0" @click="updateTeacherType" >
                        <span class="panelImage flex-grow-0 px-4">
                            <teacherIcon />
                        </span>
                        <v-divider class="me-4 my-2" vertical></v-divider>
                        <span class="panel_title text-center" v-t="'loginRegister_teacher'"></span>
                    </v-expansion-panel-header>
                </v-expansion-panel>

            </v-expansion-panels>
        </div>

        <div class="bottom">
            <div class="getStartedBottom">    
                <div class="text-center mt-3">
                    <span class="needAccount" v-t="'loginRegister_getstarted_signin_text'"></span>
                    <span class="link" v-t="'loginRegister_getstarted_signin_link'" @click="$emit('goTo', 'login')"></span>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import authMixin from '../authMixin'
import teacherIcon from '../images/teacher.svg';
import studentIcon from '../images/student.svg';

export default {
    mixins: [authMixin],
    components: {
        teacherIcon,
        studentIcon
    },
    methods: {
        closeRegister() {
            this.$store.commit('setComponent', '')
            this.$store.commit('setRequestTutor')
        },
        updateStudentType() {
            this.$emit('updateRegisterType', false)
            this.$emit('goTo', 'register')
        },
        updateTeacherType() {
            this.$emit('updateRegisterType', true)
            this.$emit('goTo', 'register')
        }
    }
}
</script>

<style lang="less">
@import '../../../../../../../styles/colors.less';
@import '../../../../../../../styles/mixin.less';

.registerType {
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    @media (max-width: @screen-xs) {
        height: 100% !important;
    }
    .top {
        .mainTitle {
            .responsive-property(font-size, 20px, null, 22px);
            color: @color-login-text-title;
            font-weight: 600;
        }
        .panels {
            margin-top: 80px;
            .panel {
                border-radius: 4px;
                border: solid 1px #b8c0d1;
                &::before, &::after {
                    box-shadow: none !important;
                    border-top: none;
                }

                .panelImage {
                    border-radius: 3px;
                    border-left: 6px solid @global-auth-text;
                    padding: 14px;
                }
                .panel_title {
                    font-size: 16px;
                    color: @global-auth-text;
                    font-weight: 600;
                }
            }
        }
    }
    .bottom {
        .getStartedBottom {
            border-top: 1px solid #dddddd;
            .responsive-property(font-size, 14px, null, 14px);
                .link {
                    cursor: pointer;  
                    color: @global-auth-text;
                    font-weight: 600;
                }
            .needAccount {
                color: @global-purple;
            }
        }
    }
}
</style>