<template>
    <component :is="component.name" :params="component.params"></component>
</template>

<script>

const auth = () => import('../../global/dialogInjection/globalDialogs/auth/auth.vue')

const simpleToaster = () => import('./simpleToaster.vue');
const simpleErrorToaster = () => import('./simpleErrorToaster.vue')
const pendingPayment = () => import('./pendingPayment.vue')
const errorLinkToaster = () => import('./errorLinkToaster.vue')

const studRoomSettings = () => import('../../../studyroom/tutorHelpers/studyRoomSettingsDialog/studyRoomSettingsDialog.vue')
const createStudyRoomDialog = () => import('../../dashboardPage/myStudyRooms/createStudyRoomDialog.vue')

export default {
    components: {
        auth,
        simpleToaster,
        simpleErrorToaster,
        pendingPayment,
        errorLinkToaster,
        studRoomSettings,
        createStudyRoomDialog,
    },
    data() {
        return {
            component: '',
            componentObj: {
                linkToaster: {
                    name: "pendingPayment",
                },
                errorToaster_notBrowser:{
                    name:'simpleErrorToaster',
                    params: {
                        text: this.$t("studyRoom_not_browser"),
                    }
                },
                errorToaster_notScreen:{
                    name:'simpleErrorToaster',
                    params: {
                        text: this.$t("studyRoom_not_screen"),
                    }
                },
                errorToaster_sessionEnded:{
                    name:'simpleErrorToaster',
                    params: {
                        text: this.$t("studyRoom_already_ended"),
                    }
                },
                errorToaster_permissionDenied:{
                    name:'errorLinkToaster',
                    params: {
                        text: this.$t('studyRoom_premission_denied',['https://support.apple.com/en-il/guide/mac-help/mchld6aa7d23/mac']),
                        timeout: 30000,
                    }
                },
                simpleToaster_userConnected:{
                    name:'simpleToaster',
                    params: {
                        text: this.$t('studyRoom_toaster_added_to_room'),
                    }
                },
                simpleToaster_userLeft:{
                    name:'simpleToaster',
                    params: {
                        text: this.$t('studyRoom_toaster_left_the_room'),
                    }
                },
                login: {
                    name: 'auth',
                    params: {
                        component: 'login',
                    }
                },
                register: {
                    name: 'auth',
                    params: {
                        component: 'register'
                    }
                },
                registerTeacher: {
                    name: 'auth',
                    params: {
                        component: 'register',
                        teacher: true
                    }
                },
                registerType: {
                    name: 'auth',
                    params: {
                        component: 'registerType'
                    }
                },
                studyRoomSettings: {
                    name: 'studRoomSettings',
                },
                createPrivateSession: {
                    name: 'createStudyRoomDialog',
                    params: {
                        type: 'private'
                    }
                },
                createLiveSession: {
                    name: 'createStudyRoomDialog',
                    params: {
                        type: 'live'
                    }
                }
            }
        }
    },
    watch: {
        "$store.getters.getComponent": "showComponent"
    },
    methods: {
        showComponent(componentName = "") {
            let componentInject = this.componentObj[componentName] || {component: '', params: ''};
            this.component = componentInject;
        }
    }
}
</script>