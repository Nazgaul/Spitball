<template>
    <component :is="component.name" :params="component.params"></component>
</template>

<script>

const simpleToaster = () => import('./simpleToaster.vue')
const pendingPayment = () => import('./pendingPayment.vue')

export default {
    components: {
        simpleToaster,
        pendingPayment
    },
    data() {
        return {
            component: '',
            toasterObj: {
                linkToaster: {
                    name: "pendingPayment",
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