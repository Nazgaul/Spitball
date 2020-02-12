<template>
    <component :is="component"></component>
</template>

<script>
import dialogComponents, {dialogConfig} from './import.js'

export default {
    mixins: [dialogComponents],
    data: () => ({
        component: ''
    }),
    watch: {
        "$route.query.dialog": "openDialog",
        getUserLoggedInStatus: {
            immediate: true,
            handler(newVal) {
                if(newVal && this.$route.query.dialog) {
                    this.openDialog(this.$route.query.dialog)
                }
            }
        }
    },
    computed: {
        getUserLoggedInStatus() {
            return this.$store.getters.getUserLoggedInStatus
        }
    },
    methods: {
        openDialog(component) {
            let dialogName;
            if(component) {
                if(dialogConfig[component].loggedPremission && this.getUserLoggedInStatus) {
                    dialogName = component;
                } else {
                    dialogName = 'login';
                }
            } else {
                dialogName = '';
            }
            this.component = dialogName;
        }
    }
}
</script>


