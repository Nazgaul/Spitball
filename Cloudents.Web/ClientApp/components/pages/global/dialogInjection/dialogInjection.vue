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
        "$route.query.dialog": "openDialog"
    },
    methods: {
        openDialog(component) {
            let c;
            if(component) {
                if(dialogConfig[component].loggedPremission) {
                    c = component;
                } else {
                    c = 'login';
                }
            } else {
                c = '';
            }
            this.component = c;
        }
    }
}
</script>


