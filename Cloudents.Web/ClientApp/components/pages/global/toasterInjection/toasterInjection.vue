<template>
    <component :is="component.name" :params="component.params"></component>
</template>

<script>

const simpleToaster = () => import('./simpleToaster.vue')
const simpleErrorToaster = () => import('./simpleErrorToaster.vue')
const pendingPayment = () => import('./pendingPayment.vue')
const errorLinkToaster = () => import('./errorLinkToaster.vue')

export default {
    components: {
        simpleToaster,
        simpleErrorToaster,
        pendingPayment,
        errorLinkToaster
    },
    data() {
        return {
            component: '',
            toasterObj: {
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
                errorToaster_permissionDenied:{
                    name:'errorLinkToaster',
                    params: {
                        text: this.$t('studyRoom_premission_denied',['https://support.apple.com/en-il/guide/mac-help/mchld6aa7d23/mac']),
                        timeout: 30000,
                    }
                }

            }
        }
    },
    watch: {
        getIsShowToaster(val = "") {
            this.showComponent(val)
        },
    },
    computed: {
        getIsShowToaster() {
            return this.$store.getters.getIsShowToaster
        }
    },
    methods: {
        showComponent(type) {
            let toaster = this.toasterObj[type] || {component: '', params: ''};
            this.component = toaster;
        }
    }
}
</script>