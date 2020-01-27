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
            this.component = component ? dialogConfig[component].loggedPremission ? component : 'login' : ''
        }
    }
}
</script>


