<template>
    <component :is="component" :component="component"></component>
</template>

<script>

const login = () => import('./login/login2.vue')
const register = () => import('./register/register.vue')

export default {
    components: {
        login,
        register
    },
    data() {
        return {
            component: '',
        }
    },
    watch: {
        "$store.getters.getLoginDialog"(val) {
            this.openDialog(val, 'login')
        },
        "$store.getters.getRegisterDialog"(val) {
            this.openDialog(val, 'register')
        },
    },
    methods: {
        openDialog(val, name) {
            if(val) {
                this.component = name
                return
            }
        }
    },
};
</script>